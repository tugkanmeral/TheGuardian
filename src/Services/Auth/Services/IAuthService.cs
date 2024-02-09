public interface IAuthService
{
    public Task<string> GetToken(AuthRequest authRequest);
}