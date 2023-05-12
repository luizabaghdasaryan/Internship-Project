using WebApp.Shared.Models;

namespace WebApp.Shared.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProducts();
        Product? GetByID(int id);
        Task<List<Product>> GetByCategory(string category);
        Task<List<string>> GetCategories();
        void Insert(Product product);
        void Update(Product product);
        void Delete(int id);
        Task<List<Product>> SearchProducts(string searchTerm);
        Task<int> CountRows();
        Task<int> CountRowsByCategory(string category);
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