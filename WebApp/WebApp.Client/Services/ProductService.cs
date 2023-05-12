using WebApp.Client.Pages;
using WebApp.Shared.Models;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;

namespace WebApp.Client.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;
        private readonly IModalService _modalService;
        private readonly IUserService _userService;
        private readonly NavigationManager _navigationManager;
        public ProductService(HttpClient httpClient, ITokenService tokenService, IModalService modalService, IUserService userService, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
            _modalService = modalService;
            _userService = userService;
            _navigationManager = navigationManager;

        }

        private static async Task<Product[]?> GetResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Product[]>();
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> AddProduct(Product product)
        {
            var token = await _tokenService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync("api/products", product);
            if (response.IsSuccessStatusCode)
            {
                _navigationManager.NavigateTo(_navigationManager.Uri, true);
                return true;    
            }
            else
            {
                await _userService.LogOutAndRemoveToken();
                _modalService.Show<LogIn>("Please Log In And Try Again");
                return false;
            }
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var token = await _tokenService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"api/products/delete/{id}");
            if(response.IsSuccessStatusCode)
            {
                _navigationManager.NavigateTo(_navigationManager.Uri, true);
                return true;
            }
            else
            {
                await _userService.LogOutAndRemoveToken();
                _modalService.Show<LogIn>("Please Log In And Try Again");
                return false;
            }
        }

        public async Task<bool> EditProduct(Product product)
        {
            var token = await _tokenService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PutAsJsonAsync("api/products", product);
            if (response.IsSuccessStatusCode)
            {
                _navigationManager.NavigateTo(_navigationManager.BaseUri, true);
                return true;
            }
            else
            {
                await _userService.LogOutAndRemoveToken();
                _modalService.Show<LogIn>("Please Log In And Try Again");
                return false;
            }
        }

        public async Task<Product[]?> GetAllProducts()
        {
            return await _httpClient.GetFromJsonAsync<Product[]>("api/products");
        }

        public async Task<string[]?> GetCategories()
        {
            return await _httpClient.GetFromJsonAsync<string[]?>("api/products/categories");
        }

        public async Task<Product[]?> GetNext()
        {
            var response = await _httpClient.GetAsync("api/products/next");
            return await GetResponse(response);
        }

        public async Task<Product[]?> GetNextByCategory(string category)
        {
            var response = await _httpClient.GetAsync($"api/products/{category}/next");
            return await GetResponse(response);
        }

        public async Task<Product[]?> GetPrevious()
        {
            var response =  await _httpClient.GetAsync("api/products/previous");
            return await GetResponse(response);
        }

        public async Task<Product[]?> GetPreviousByCategory(string category)
        {
            var response =  await _httpClient.GetAsync($"api/products/{category}/previous");
            return await GetResponse(response); 
        }

        public async Task<Product?> GetProductByID(string id)
        {
            var response = await _httpClient.GetAsync($"api/products/id/{id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadFromJsonAsync<Product>();
            }
            else
            {
                return null;
            }
        }

        public async Task<Product[]?> GetProductsByCategory(string category)
        {
            var response = await _httpClient.GetAsync($"api/products/ByCategory/{category}");
            return await GetResponse(response);
        }

        public async Task<Product[]?> SearchProducts(string searchTerm)
        {
            var response = await _httpClient.GetAsync($"api/products/search?search={searchTerm}");
            return await GetResponse(response); 
        }

        public async Task<Product[]?> GetNextBySearch(string searchTerm)
        {
            var response = await _httpClient.GetAsync($"api/products/search/next?search={searchTerm}");
            return await GetResponse(response);
        }

        public async Task<Product[]?> GetPreviousBySearch(string searchTerm)
        {
            var response = await _httpClient.GetAsync($"api/products/search/previous?search={searchTerm}");
            return await GetResponse(response);
        }

        public async Task<Product[]?> GoToPage(int page)
        {
            var response = await _httpClient.GetAsync($"api/products/page/{page}");
            return await GetResponse(response);
        }

        public async Task<Product[]?> GoToPageByCategory(string category, int page)
        {
            var response = await _httpClient.GetAsync($"api/products/{category}/{page}");
            return await GetResponse(response);
        }

        public async Task<Product[]?> GoToPageBySearch(string searchTerm, int page)
        {
            var response = await _httpClient.GetAsync($"api/products/search/{page}?search={searchTerm}");
            return await GetResponse(response);
        }

        public async Task<int> GetPageCount()
        {
            return await _httpClient.GetFromJsonAsync<int>("api/products/PageCount");
        }

        public async Task<int> GetPageCountByCategory(string category)
        {
            return await _httpClient.GetFromJsonAsync<int>($"api/products/{category}/PageCount");
        }

        public async Task<int> GetPageCountBySearch(string searchTerm)
        {
            return await _httpClient.GetFromJsonAsync<int>($"api/products/search/PageCount?search={searchTerm}");
        }
    }
}