using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("EmployeeBusiness", policy => policy.RequireRole("Employee"));

var app = builder.Build();

app.MapGet("/api/employee", () =>
{
    return new Employee("Tuğkan", "Meral", Gender.Male, new DateOnly(1992, 11, 8), Guid.Parse("96c86995-434a-42c3-a84b-37c1ebdb3461"));
});

app.MapGet("/api/employee/test", (string value) =>
{
    return value;
}).RequireAuthorization("EmployeeBusiness");

app.UseAuthentication();
app.UseAuthorization();

app.Run();

record Employee(string name, string surname, Gender gender, DateOnly birthDate, Guid userId);

enum Gender
{
    Female,
    Male,
    Other
}