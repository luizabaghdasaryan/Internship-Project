using WebApp.Client.Authentication;
using WebApp.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace WebApp.Client.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;
        private readonly NavigationManager _navigationManager;
        private AuthenticationStateProvider _customAuthenticationStateProvider;
        public UserService(HttpClient httpClient, ITokenService tokenService, NavigationManager navigationManager, AuthenticationStateProvider customAuthenticationStateProvider)
        {
            _tokenService = tokenService;
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _customAuthenticationStateProvider = customAuthenticationStateProvider;
        }
        public async Task<bool> LoggedInAndTokenStored(LoginUser user)
        {
            var response = await _httpClient.PostAsJsonAsync("api/login", user);
            if (response.IsSuccessStatusCode)
            {
                string tokenGenerated = await response.Content.ReadAsStringAsync();
                await _tokenService.SetTokenAsync(tokenGenerated);
                ((CustomAuthenticationProvider)_customAuthenticationStateProvider).Notify();
                _navigationManager.NavigateTo("/");
                return true;
            }
            return false;
        }
        public async Task<bool> RegisteredAndTokenStored(User user)
        {
            var response = await _httpClient.PostAsJsonAsync("api/createaccount", user);
            if (response.IsSuccessStatusCode)
            {
                string tokenGenerated = await response.Content.ReadAsStringAsync();
                await _tokenService.SetTokenAsync(tokenGenerated);
                ((CustomAuthenticationProvider)_customAuthenticationStateProvider).Notify();
                _navigationManager.NavigateTo("/");
                return true;
            }
            return false;
        }
        public async Task LogOutAndRemoveToken()
        {
            await _tokenService.RemoveTokenAsync();
            ((CustomAuthenticationProvider)_customAuthenticationStateProvider).Notify();
        }
    }
}
