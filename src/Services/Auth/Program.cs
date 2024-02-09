using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(x => x.UseInMemoryDatabase("TheGuardianDB"));
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

app.MapPost("/api/getToken", async (AuthRequest authRequest) =>
{
    try
    {
        using var scope = app.Services.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
        var token = await authService.GetToken(authRequest);

        if (String.IsNullOrEmpty(token))
            return Results.BadRequest("Check your credentials!");

        return Results.Ok(token);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex.Message);
        return Results.Problem("An error occurred!");
    }
});

app.Run();