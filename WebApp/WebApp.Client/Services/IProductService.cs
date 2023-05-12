using WebApp.Shared.Models;
namespace WebApp.Client.Services
{
    public interface IProductService
    {
        Task<Product[]?> GetAllProducts();
        Task<Product?> GetProductByID(string id);
        Task<string[]?> GetCategories();
        Task<Product[]?> GetProductsByCategory(string category);
        Task<bool> DeleteProduct(int id);
        Task<bool> AddProduct(Product product);
        Task<bool> EditProduct(Product product);
        Task<Product[]?> SearchProducts(string searchTerm);
        Task<Product[]?> GetNext();
        Task<Product[]?> GetPrevious();
        Task<Product[]?> GetNextByCategory(string category);
        Task<Product[]?> GetPreviousByCategory(string category);
        Task<Product[]?> GetNextBySearch(string searchTerm);
        Task<Product[]?> GetPreviousBySearch(string searchTerm);
        Task<Product[]?> GoToPage(int page);
        Task<Product[]?> GoToPageByCategory(string category, int page);
        Task<Product[]?> GoToPageBySearch(string searchTerm, int page);
        Task<int> GetPageCount();
        Task<int> GetPageCountByCategory(string category);
        Task<int> GetPageCountBySearch(string searchTerm);
    }
}
