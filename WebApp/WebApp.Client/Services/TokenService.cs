using Blazored.SessionStorage;

namespace WebApp.Client.Services
{
    public class TokenService : ITokenService
    {
        private readonly ISessionStorageService _sessionStorageService;
        public TokenService(ISessionStorageService sessionStorageService)
        {
            _sessionStorageService = sessionStorageService;
        }
        public async Task<string> GetTokenAsync()
        {
            return await _sessionStorageService.GetItemAsStringAsync("accessToken");
        }
        public async Task RemoveTokenAsync()
        {
            await _sessionStorageService.RemoveItemAsync("accessToken");
        }
        public async Task SetTokenAsync(string token)
        {
            await _sessionStorageService.SetItemAsStringAsync("accessToken", token);
        }
    }
}
