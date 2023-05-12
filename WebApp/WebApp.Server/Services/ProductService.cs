using WebApp.Shared.Models;
using WebApp.Shared.Repositories;

namespace WebApp.Shared.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllProducts();
        }

        public async Task<bool> IsValidID(int id)
        {
            return await Task.FromResult(_productRepository.GetByID(id) != null);
        }

       public async Task<bool> IsExistingCategory(string category)
        {
            var products = await _productRepository.GetByCategory(category);
            return products.Count != 0;
        }

        public async Task<List<Product>> GetByCategoryAsync(string category)
        {
            return await _productRepository.GetByCategory(category);
        }

        public async Task<Product?> GetByIDAsync(int id)
        {
            return await Task.FromResult(_productRepository.GetByID(id));
        }

        public async Task DeleteAsync(int id)
        {
           await Task.Run(() => _productRepository.Delete(id));
        }

        public async Task InsertAsync(Product product)
        {
            await Task.Run(() => _productRepository.Insert(product));
        }

        public async Task UpdateAsync(Product product)
        {
            await Task.Run(() => _productRepository.Update(product));
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            return await _productRepository.GetCategories();
        }

        public async Task<List<Product>?> GetNextProducts()
        {
            return await _productRepository.GetNextProducts();
        }

        public async Task<List<Product>?> GetPreviousProducts()
        {
            return await _productRepository.GetPreviousProducts();
        }

        public async Task<List<Product>?> GetNextProductsByCategory(string category)
        {
            return await _productRepository.GetNextProductsByCategory(category);
        }

        public async Task<List<Product>?> GetPreviousProductsByCategory(string category)
        {
            return await _productRepository.GetPreviousProductsByCategory(category);
        }

        public async Task<List<Product>?> SearchProducts(string searchTerm)
        {
            return await _productRepository.SearchProducts(searchTerm);
        }

        public async Task<List<Product>?> GetNextProductsBySearch(string searchTerm)
        {
            return await _productRepository.GetNextProductsBySearch(searchTerm);
        }

        public async Task<List<Product>?> GetPreviousProductsBySearch(string searchTerm)
        {
            return await _productRepository.GetPreviousProductsBySearch(searchTerm);
        }

        public async Task<List<Product>?> GoToPage(int page)
        {
            return await _productRepository.GoToPage(page);
        }

        public async Task<List<Product>?> GoToPageByCategory(string category, int page)
        {
            return await _productRepository.GoToPageByCategory(category, page);
        }

        public async Task<List<Product>?> GoToPageBySearch(string searchTerm, int page)
        {
            return await _productRepository.GoToPageBySearch(searchTerm, page);
        }

        public async Task<int> GetPageCount()
        {
            return await _productRepository.GetPageCount();
        }

        public async Task<int> GetPageCountByCategory(string category)
        {
            return await _productRepository.GetPageCountByCategory(category);
        }

        public async Task<int> GetPageCountBySearch(string searchTerm)
        {
            return await _productRepository.GetPageCountBySearch(searchTerm);
        }
    }
}