using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebAPI_ZitaDel;
using Zitadel.Authentication;
using Zitadel.Authentication.Options;
using Zitadel.Credentials;
using Zitadel.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var zitaDelAuthentication = builder.Configuration.GetSection("ZitaDelJWTKeyConfig");

//builder.Services
//    .AddAuthorization()
//    .AddAuthentication()
//    .AddZitadelIntrospection("ZITADEL_JWT", o =>
//        {
//            o.Authority = "https://connect-idp-service-bmr36j.us1.zitadel.cloud";
//            o.JwtProfile = Application.LoadFromJsonString(
//                @"
//{
//  ""type"": ""application"",
//  ""keyId"": ""286357374938183559"",
//  ""key"": ""-----BEGIN RSA PRIVATE KEY-----\nMIIEogIBAAKCAQEA7iLK9yyo+QC+dlw7FamFo0e4GYiC/Gd2ZKwD7+feVsAAiN9P\nuwnJpnKZAVZBanw1HjdxheCuBl4+iPIJnme+2RVSuToC0KFGPj5/WsceoAtdXP0i\nJr43NSVroANTBTHOkdxoN/LsZLkC0q7RUUqnl/gOqe5UTE7eDfdic2c4lkXAScNZ\nI2Xvv7NSA1mTbQF1qCcCz7WbW2QDNTNVBFNvRiuUqNtKnrIF3DunLKhT7ehHqxWL\noWfnwdG4ItwpyTSiRQXFPRmQsiZji0l2VjYch0VUvPg84PAvXQjDKGjf5KOrQdg6\nc9SgEtgXGK2zVjh0SD0NM78OXZ4RhZQw6+FbwwIDAQABAoIBAEpY6H9FHTgoiE5R\nu0ynEYcJxCuMmECPWMJThiMnhB4XcdNJzQ27H47s5mIpeODzCG53puTCYfKxB9sf\nnesrvsTtyFb2CpzyTQjv4DkKQ8B569s1WK054+ie1zws7YWIqFKXup4FJNLJhF0c\noVM8+JgUpRdexkjQsrkvjDg1eNBJOm+SSB0GNEKyFukNfkEn1ybdOJ9EMcY4PfO/\nQleoEGXj93g+6x20BvwHo1pd6NqZFlMprpWSdTKiiPANVKD5nFwExr5SFxOmF913\nHxZ5gJAgnWZqCmnPRvTduaG3W7S/T8zEu3YFsN+WtUicVgU3fjbejGooY2fx474d\neOdDVXkCgYEA+rRZK1eOiJsmSy4+FO2nvbj4DXqlvKZO4G/JoE1sLY9D9qSUVOHX\nGQTn/We8iIeilViypDLoo6XGhkZhWVSDvfmPbuld1wt1UF3riKzCTdVlUJNMc6Un\nlm5Ij9b6BxrOgq4kqn2jlEH1fbNqqR0HqIGxFVdHSaCDXpt2K7s1x2UCgYEA8yp7\nSUSWQ4+e74vsBXCF5KRYyoLujRBd9rfI/kI9cs+CsbHe8kbYCfR1Y8ETpCggucnC\nbu3dYZkmkCD9bUwpsx3EpnzcbPIcCvAFzVysfAkHWJ2it6GulrQptGHflWl+OJQL\nioWvcJGE44xAt6cC1eFjGgvtRT3B6G02ET36yAcCgYAZn1kFrvsoRwMQ5/b8WJWu\nNDtV1VUfBLhHA+XNdlw6A8xkZsmqKzK6od+77GA+a+5X6SrI0VMpdoXlr0e/w317\nawgXw84NbmRdBFxQKafqIIcsRwGnSBfVrgvId6YyF7FHddjVe+W9/0fBwxaBli/0\nLt0UW0kz4y3J+4WGTqneaQKBgEQPJNiZ+NDKFvsHF7ViDCe1lCCCSxByf0o5oGkB\n0z8aM4NWvIblKzyGDoEwHjY5AQffH6t6qhbSDcgTY7s+sQEwGMVcdzmK/ixVh9o1\n0RUaVdMIs+1ftIT7whx5tGGWWDyvuxc3WaZRJHen3slIVnPUlK9KnUtfALH0N0jW\nJyXrAoGATii7ZsvhecF9xRMI5fr4S1r5lcj1CTkyE//LA+fPPBpi/Gf/uhrvQHGe\n9EDOk848CIMS22QsM4Kt8k+cbL51NQ86IFFx37qrrmQ2zXR9DmFx3itBdITlM867\nGB+4qGbYSL/rn9HUCyMJsx6SX9bSA8CiBiWkytu9GSUW5hrPztw=\n-----END RSA PRIVATE KEY-----\n"",
//  ""appId"": ""286357330411452295"",
//  ""clientId"": ""286357330411517831""
//}");
//        });

var path = @"C:\Users\Shankargoud.Patil.CROWCON\Downloads\286357374938183559.json";

ZitaDelApplication zitaDelApplication = ZitaDelApplication.LoadFromJsonFile(path);

builder.Services.AddAuthentication()
    .AddOAuth2Introspection("Introspection", options =>
    {
        options.Authority = "https://connect-idp-service-bmr36j.us1.zitadel.cloud";
        //options.ClientId = "286357330411517831";
        options.ClientId = null;
        options.ClientSecret = null;
        options.ClientCredentialStyle = ClientCredentialStyle.PostBody;
        options.Events.OnUpdateClientAssertion += async context =>
        {
            var jwt = await zitaDelApplication.GetSignedJwtAsync(options.Authority);
            context.ClientAssertion = new()
            {
                Type = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer",
                Value = jwt,
            };
            context.ClientAssertionExpirationTime = DateTime.UtcNow.AddMinutes(55);
        };
    });

builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();

