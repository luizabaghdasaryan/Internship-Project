using WebApp.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebApp.Client.Authentication
{
    public class CustomAuthenticationProvider : AuthenticationStateProvider
    {
        private ITokenService _tokenService;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        public CustomAuthenticationProvider(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _tokenService.GetTokenAsync();
            if (token == null) 
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            string name = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            string email = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            string role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> 
            { 
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role) 
            }, "JwtAuth"));
            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }
        public void Notify()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
