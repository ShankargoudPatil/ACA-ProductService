using System.Security.Claims;
using Application;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using WebApi;
using WebApi.GraphQL;
using OpenTelemetry.Logs;
using Google.Api;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Kinde.Api.Model;
using System;
using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
//Add SeriLog to the container
const string serviceName = "product-service";
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom
    .Configuration(hostingContext.Configuration);
});

// Add services to the container.
builder.Services.AddApplication().AddInfrastructure();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDbContext<ApplicationSqlServerSqlDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));
builder.Services.AddScoped<IClaimsTransformation, CustomClaimsTransformer>();
builder.Services.AddDbContextPool<ApplicationPGSqlDbContext>(
    option =>
    {
        option.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection"));
    });

var b2CTenantConfig = builder.Configuration.GetSection("B2CTenantConfig");
var zitaDelAuthentication = builder.Configuration.GetSection("ZitaDelAuthentication");

builder.Services.AddAuthentication(options =>
{
    // Don't set default schemes, handle them manually using PolicyScheme
    options.DefaultScheme = "DynamicScheme"; // Set the default scheme to the dynamic policy scheme
})
.AddJwtBearer("ZitaDelBearer", options =>
{
    // Configure Zitadel JWT validation
    zitaDelAuthentication.Bind("JwtBearer", options);
    options.Authority = zitaDelAuthentication["JwtBearer:Authority"];
    options.Audience = zitaDelAuthentication["JwtBearer:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = zitaDelAuthentication["JwtBearer:Authority"],
        ValidAudience = zitaDelAuthentication["JwtBearer:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = ClaimTypes.Role // Ensure role claims are properly handled
    };
})
.AddOAuth2Introspection("Introspection", options =>
{
    // Configure OAuth2 introspection for opaque tokens
    options.Authority = "https://connect-idp-service-bmr36j.us1.zitadel.cloud";
    options.ClientId = "12862131573687561031";
    options.ClientSecret = "hd2kezw4DIPBZGcuExS7SuZ4ituymiNTXckAbq2ojsB6qBlhQxvgt8jsUernyf301";
    options.IntrospectionEndpoint = "https://connect-idp-service-bmr36j.us1.zitadel.cloud/oauth/v2/introspect";
    options.RoleClaimType = ClaimTypes.Role;// Handle roles in opaque tokens
})
.AddJwtBearer("ClientCredentials", options =>
{
    // Configure Client Credentials flow validation
    options.Authority = "https://connect-idp-service-bmr36j.us1.zitadel.cloud";
    options.Audience = "ApiUser";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
})
.AddPolicyScheme("DynamicScheme", "Token Scheme", options =>
{
    // Dynamically select the authentication scheme based on the token format
    options.ForwardDefaultSelector = context =>
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();

            // JWT: Contains two dots (header, payload, signature)
            if (token.Count(c => c == '.') == 2)
            {
                var jwtHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                if (jwtHandler.CanReadToken(token))
                {
                    var jwtToken = jwtHandler.ReadJwtToken(token);

                    // Check the issuer to decide the token origin (Azure B2C or Zitadel)
                   if (jwtToken.Audiences.Contains("ApiUser"))
                    {
                        return "ClientCredentials"; // Use Zitadel scheme
                    }
                    else if (jwtToken.Issuer.Contains("zitadel"))
                    {
                        return "ZitaDelBearer"; // Use Zitadel scheme
                    }
                }
            }
            else
            {
                // Opaque token: use introspection
                return "Introspection";
            }
        }

        return "Bearer"; // No valid token found, no forwarding
    };
})
.AddMicrosoftIdentityWebApi(options =>
{
    b2CTenantConfig.Bind("AzureAdB2C", options);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidAudience = b2CTenantConfig["AzureAdB2C:ClientId"],
        ValidIssuer = b2CTenantConfig["AzureAdB2C:Issuer"],
    };
    //  options.TokenValidationParameters.NameClaimType = "name";
    //  options.SaveToken = true;
},
                   options => { b2CTenantConfig.Bind("AzureAdB2C", options); },
                   JwtBearerDefaults.AuthenticationScheme, true);


builder.Services.AddAuthorization(options =>
{

    options.AddPolicy("AtLeast21", policy =>
        policy.Requirements.Add(new MinimumAgeRequirement("AtLeast21")));

    options.AddPolicy("HasCountry", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == ClaimTypes.Country)));
});
builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();

builder.Services.AddControllers().AddDapr();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ProductQuery>();
builder.Services.AddGraphQLServer()
    .AddAuthorization()
    .RegisterService<ProductRepository>(ServiceKind.Resolver)
    // .RegisterDbContext<ApplicationPGSqlDbContext>()
    .AddQueryType<ProductQuery>()
    .AddDataLoader<ProductBatchDataLoader>()
    .AddFiltering();

//open telemetry
//add zipkin exporter

//builder.Logging.AddOpenTelemetry(options =>
//{
//    options
//        .SetResourceBuilder(
//            ResourceBuilder.CreateDefault().AddService(serviceName))
//        .AddConsoleExporter();
//});

//builder.Services.AddOpenTelemetry()
//      .ConfigureResource(resource => resource.AddService(serviceName))
//      .WithTracing(tracing => tracing
//          .AddAspNetCoreInstrumentation()
//          .AddZipkinExporter()
//          .AddConsoleExporter())
//      .WithMetrics(metrics => metrics
//          .AddAspNetCoreInstrumentation()
//          .AddConsoleExporter());



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGraphQL("/graphql");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationPGSqlDbContext>();
    dbContext.Database.Migrate();

    var sqlDbContext = scope.ServiceProvider.GetRequiredService<ApplicationSqlServerSqlDbContext>();
    sqlDbContext.Database.Migrate();

}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
