using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthentication("Bearer")
       .AddOAuth2Introspection("Bearer", options =>
       {
           options.Authority = "https://connect-idp-service-bmr36j.us1.zitadel.cloud";
           options.ClientId = "286213157368756103"; // Replace with your API client ID
           options.ClientSecret = "hd2kezw4DIPBZGcuExS7SuZ4ituymiNTXckAbq2ojsB6qBlhQxvgt8jsUernyf30"; // Replace with your API client secret
           options.IntrospectionEndpoint = "https://connect-idp-service-bmr36j.us1.zitadel.cloud/oauth/v2/introspect";
           options.RoleClaimType = ClaimTypes.Role;
       });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthorization();
app.MapControllers();

app.Run();
