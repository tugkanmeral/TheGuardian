using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

public class AuthService(IConfiguration configuration, DatabaseContext databaseContext) : IAuthService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly DatabaseContext _dbContext = databaseContext;

    public async Task<string> GetToken(AuthRequest authRequest)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == authRequest.UserName);
        if (user is null)
            return String.Empty;

        StringBuilder rawPassStrBuilder = new();
        rawPassStrBuilder.Append(authRequest.UserName);
        rawPassStrBuilder.Append(authRequest.Password);
        string rawPass = rawPassStrBuilder.ToString();

        var passHash = GetHash(rawPass);

        if (user.PasswordHash != passHash)
            return String.Empty;

        var token = CreateToken(user);
        return token;
    }

    private string CreateToken(User user)
    {
        var expiration = DateTime.UtcNow.AddMinutes(120);
        var token = CreateJwtToken(
            CreateClaims(user),
            CreateSigningCredentials(),
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.WriteToken(token);
    }

    private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
        DateTime expiration) =>
        new(
            _configuration.GetValue<string>("Auth:Issuer"),
            _configuration.GetValue<string>("Auth:ValidAudience"),
            claims,
            expires: expiration,
            signingCredentials: credentials
        );

    private List<Claim> CreateClaims(User user)
    {
        var roles = GetUserRoles(user);

        try
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName)
            };

            roles.ForEach((role) =>
            {
                claims.Add(new(ClaimTypes.Role, role));
            });

            return claims;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public List<string> GetUserRoles(User user)
    {
        return ["Employee"];
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var symmetricSecurityKey = _configuration.GetValue<string>("Auth:SymmetricSecurityKey");

        ArgumentNullException.ThrowIfNull(symmetricSecurityKey);

        return new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(symmetricSecurityKey)
            ),
            SecurityAlgorithms.HmacSha256
        );
    }

    private static string GetHash(string input)
    {
        byte[] data = SHA256.HashData(Encoding.UTF8.GetBytes(input));

        var sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
            sBuilder.Append(data[i].ToString("x2"));

        return sBuilder.ToString();
    }
}