using WebApp.Shared.Models;

namespace WebApp.Shared.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<bool> IsValidID(int id);
        Task<bool> IsExistingCategory(string category);
        Task<Product?> GetByIDAsync(int id);
        Task<List<Product>> GetByCategoryAsync(string category);
        Task<List<string>> GetCategoriesAsync();
        Task InsertAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<List<Product>?> SearchProducts(string searchTerm);
        Task<List<Product>?> GetNextProducts();
        Task<List<Product>?> GetPreviousProducts();
        Task<List<Product>?> GetNextProductsByCategory(string category);
        Task<List<Product>?> GetPreviousProductsByCategory(string category);
        Task<List<Product>?> GetNextProductsBySearch(string searchTerm);
        Task<List<Product>?> GetPreviousProductsBySearch(string searchTerm);
        Task<List<Product>?> GoToPage(int page);
        Task<List<Product>?> GoToPageByCategory(string category, int page);
        Task<List<Product>?> GoToPageBySearch(string searchTerm, int page);
        Task<int> GetPageCount();
        Task<int> GetPageCountByCategory(string category);
        Task<int> GetPageCountBySearch(string searchTerm);
    }
}