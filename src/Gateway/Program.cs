using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var symmetricSecurityKey = builder.Configuration.GetValue<string>("Auth:SymmetricSecurityKey");
ArgumentNullException.ThrowIfNull(symmetricSecurityKey);

var issuer = builder.Configuration.GetValue<string>("Auth:Issuer");
ArgumentNullException.ThrowIfNull(issuer);

var validAudience = builder.Configuration.GetValue<string>("Auth:ValidAudience");
ArgumentNullException.ThrowIfNull(validAudience);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.IncludeErrorDetails = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = validAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(symmetricSecurityKey)
            ),
        };
    });

builder.Configuration.AddJsonFile("ocelotConfig.json");
builder.Services.AddOcelot();

var app = builder.Build();

app.UseOcelot().Wait();

app.UseAuthentication();
app.UseAuthorization();

app.Run();