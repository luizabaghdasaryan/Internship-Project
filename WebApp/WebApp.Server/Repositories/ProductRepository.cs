using System.Data.SqlClient;
using WebApp.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Shared.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string connectionString;
        private static int offset1 = 0;
        private static int offset2 = 0;
        public int itemsPerPage { get; } = 10;
        public ProductRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DbConnection");
        }
        public Task<List<Product>> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT Product.ID, Product.Name, Product.Brand, Category.Name as Category FROM Product INNER JOIN Category ON Product.Category_ID = Category.ID", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ID = (int)reader["ID"],
                        Name = $"{reader["Name"]}",
                        Brand = $"{reader["Brand"]}",
                        Category = $"{reader["Category"]}"
                    });
                }
                return Task.FromResult(products);
            }
        }

        public Task<List<Product>> GetByCategory(string category)
        {
            List<Product> products = new List<Product>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT Product.ID, Product.Name, Product.Brand, Category.Name as Category FROM Product INNER JOIN Category ON Product.Category_ID = Category.ID WHERE Category.Name = @Category", connection);
                command.Parameters.AddWithValue("@Category", category);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ID = (int)reader["ID"],
                        Name = $"{reader["Name"]}",
                        Brand = $"{reader["Brand"]}",
                        Category = $"{reader["Category"]}"
                    });
                }
                return Task.FromResult(products);
            }
        }

        public Product? GetByID(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT Product.ID, Product.Name, Product.Brand, Category.Name AS Category FROM Product INNER JOIN Category ON Product.Category_ID = Category.ID WHERE Product.ID = @pID", connection);
                command.Parameters.AddWithValue("@pID", $"{id}");
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    return new Product { ID = (int)reader["ID"], Name = $"{reader["Name"]}", Brand = $"{reader["Brand"]}", Category = $"{reader["Category"]}" };
                }
                return null;
            }
        }

        public void Insert(Product p)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("INSERT INTO Product (Name, Brand, Category_ID) " +
                                                  "VALUES (@pName, @pBrand, (SELECT ID FROM Category WHERE Name = @pCategory))", connection);
                command.Parameters.AddWithValue("@pName", $"{p.Name}");
                command.Parameters.AddWithValue("@pBrand", $"{p.Brand}");
                command.Parameters.AddWithValue("@pCategory", $"{p.Category}");
                command.ExecuteNonQuery();
            }
        }

        public void Update(Product p)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("UPDATE Product SET Name = @pName, Brand = @pBrand, Category_ID = (SELECT ID FROM Category WHERE Name = @pCategory) WHERE ID = @pID", connection);
                command.Parameters.AddWithValue("@pID", $"{p.ID}");
                command.Parameters.AddWithValue("@pName", $"{p.Name}");
                command.Parameters.AddWithValue("@pBrand", $"{p.Brand}");
                command.Parameters.AddWithValue("@pCategory", $"{p.Category}");
                command.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("DELETE FROM Product WHERE ID = @pID", connection);
                command.Parameters.AddWithValue("@pID", $"{id}");
                command.ExecuteNonQuery();
            }
        }

        public Task<List<string>> GetCategories()
        {
            List<string> categories = new List<string>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT Name FROM Category", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    categories.Add($"{reader["Name"]}");
                }
                return Task.FromResult(categories);
            }
        }

        //to get only 10 products at every page
        public async Task<List<Product>?> GetNextProducts()
        {
            List<Product> products = new List<Product>();
            using (var connection = new SqlConnection(connectionString))
            {
                var count = await CountRows();
                if (count == 0)
                {
                    return null;
                }
                if (offset1 >= count)
                {
                    offset1 -= itemsPerPage;
                }
                if (offset1 - offset2 > itemsPerPage)
                {
                    offset2 = offset1 - itemsPerPage;
                }
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT Product.ID, Product.Name, Product.Brand, Category.Name AS Category FROM Product INNER JOIN Category ON Product.Category_ID = Category.ID ORDER BY Product.ID offset @_offset rows fetch next @_next rows only", connection);
                command.Parameters.AddWithValue("@_offset", offset1);
                command.Parameters.AddWithValue("@_next", itemsPerPage);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ID = (int)reader["ID"],
                        Name = $"{reader["Name"]}",
                        Brand = $"{reader["Brand"]}",
                        Category = $"{reader["Category"]}"
                    });
                }
                offset1 += itemsPerPage;
                return products;
            }
        }

        public async Task<List<Product>?> GetPreviousProducts()
        {
            List<Product> products = new List<Product>();
            using (var connection = new SqlConnection(connectionString))
            {
                var count = await CountRows();
                if (count == 0)
                {
                    return null;
                }
                if (offset2 < 0)
                {
                    offset2 = 0;
                }
                if (offset1 - offset2 > itemsPerPage)
                {
                    offset1 = offset2 + itemsPerPage;
                }
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT Product.ID, Product.Name, Product.Brand, Category.Name AS Category FROM Product INNER JOIN Category ON Product.Category_ID = Category.ID ORDER BY Product.ID offset @_offset rows fetch next @_next rows only", connection);
                command.Parameters.AddWithValue("@_offset", offset2);
                command.Parameters.AddWithValue("@_next", itemsPerPage);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ID = (int)reader["ID"],
                        Name = $"{reader["Name"]}",
                        Brand = $"{reader["Brand"]}",
                        Category = $"{reader["Category"]}"
                    });
                }
                offset2 -= itemsPerPage;
                return products;
            }
        }

        //get the number of products
        public Task<int> CountRows()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT Count(*) FROM Product", connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                }
                return Task.FromResult((int)reader[0]);
            }
        }

        //to get only 10 products at every page of the selected category
        public async Task<List<Product>?> GetNextProductsByCategory(string category)
        {
            List<Product> products = new List<Product>();
            using (var connection = new SqlConnection(connectionString))
            {
                var count = await CountRowsByCategory(category);
                if (count == 0)
                {
                    return null;
                }
                if (offset1 >= count)
                {
                    offset1 -= itemsPerPage;
                }
                if (offset1 - offset2 > itemsPerPage)
                {
                    offset2 = offset1 - itemsPerPage;
                }
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT Product.ID, Product.Name, Product.Brand, Category.Name AS Category FROM Product INNER JOIN Category ON Product.Category_ID = Category.ID WHERE Category.Name = @_category ORDER BY Product.ID offset @_offset rows fetch next @_next rows only", connection);
                command.Parameters.AddWithValue("@_offset", offset1);
                command.Parameters.AddWithValue("@_next", itemsPerPage);
                command.Parameters.AddWithValue("@_category", category);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ID = (int)reader["ID"],
                        Name = $"{reader["Name"]}",
                        Brand = $"{reader["Brand"]}",
                        Category = $"{reader["Category"]}"
                    });
                }
                offset1 += itemsPerPage;
                return products;
            }
        }

        public async Task<List<Product>?> GetPreviousProductsByCategory(string category)
        {
            List<Product> products = new List<Product>();
            using (var connection = new SqlConnection(connectionString))
            {
                var count = await CountRowsByCategory(category);
                if (count == 0)
                {
                    return null;
                }
                if (offset2 < 0)
                {
                    offset2 = 0;
                }
                if (offset1 - offset2 > itemsPerPage)
                {
                    offset1 = offset2 + itemsPerPage;
                }
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT Product.ID, Product.Name, Product.Brand, Category.Name AS Category FROM Product INNER JOIN Category ON Product.Category_ID = Category.ID WHERE Category.Name = @_category ORDER BY Product.ID offset @_offset rows fetch next @_next rows only", connection);
                command.Parameters.AddWithValue("@_offset", offset2);
                command.Parameters.AddWithValue("@_next", itemsPerPage);
                command.Parameters.AddWithValue("@_category", category);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ID = (int)reader["ID"],
                        Name = $"{reader["Name"]}",
                        Brand = $"{reader["Brand"]}",
                        Category = $"{reader["Category"]}"
                    });
                }
                offset2 -= itemsPerPage;
                return products;
            }
        }

        //get the number of products of the selected category
        public Task<int> CountRowsByCategory(string category)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT Count(*) FROM Product INNER JOIN Category ON Product.Category_ID = Category.ID WHERE Category.Name = @_category", connection);
                command.Parameters.AddWithValue("@_category", category);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                }
                return Task.FromResult((int)reader[0]);
            }
        }

        public Task<List<Product>> SearchProducts(string searchTerm)
        {
            List<Product> products = new List<Product>();
            List<string> searchKeys = searchTerm.Split(' ').ToList<string>();
            foreach (var key in searchKeys)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT Product.ID, Product.Name, Product.Brand, Category.Name AS Category FROM Product INNER JOIN Category ON Product.Category_ID = Category.ID WHERE Product.Name LIKE @key_location1 OR Product.Name LIKE @key_location2 OR Product.Name LIKE @key_location3 OR Product.Name Like @key_location4 OR Product.Brand LIKE @key_location1 OR Product.Brand LIKE @key_location2 OR Product.Brand LIKE @key_location3 OR Product.Brand LIKE @key_location4 OR Category.Name LIKE @key_location1 OR Category.Name LIKE @key_location2 OR Category.Name LIKE @key_location3 OR Category.Name LIKE @key_location4", connection);
                    command.Parameters.AddWithValue("@key_location1", $"{key}");
                    command.Parameters.AddWithValue("@key_location2", $"{key} %");
                    command.Parameters.AddWithValue("@key_location3", $"% {key}");
                    command.Parameters.AddWithValue("@key_location4", $"% {key} %");
                    SqlDataReader reader = command.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        break;
                    }
                    else
                    {
                        if (products.IsNullOrEmpty())
                        {
                            while (reader.Read())
                            {
                                products.Add(new Product
                                {
                                    ID = (int)reader["ID"],
                                    Name = $"{reader["Name"]}",
                                    Brand = $"{reader["Brand"]}",
                                    Category = $"{reader["Category"]}"
                                });
                            }
                        }
                        else
                        {
                            List<Product> _products = new List<Product>();
                            while (reader.Read())
                            {
                                _products.Add(new Product
                                {
                                    ID = (int)reader["ID"],
                                    Name = $"{reader["Name"]}",
                                    Brand = $"{reader["Brand"]}",
                                    Category = $"{reader["Category"]}"
                                });
                            }
                            products = products.Intersect(_products).ToList();
                        }
                    }
                }
            }
            return Task.FromResult(products);
        }

        public async Task<List<Product>?> GetNextProductsBySearch(string searchTerm)
        {
            List<Product>? products = await SearchProducts(searchTerm);
            if (products.IsNullOrEmpty())
            {
                return null;
            }
            var count = products.Count;
            if (offset1 >= count)
            {
                offset1 -= itemsPerPage;
            }
            if (offset1 - offset2 > itemsPerPage)
            {
                offset2 = offset1 - itemsPerPage;
            }
            var _products = products.Skip(offset1).Take(itemsPerPage).ToList();
            offset1 += itemsPerPage;
            return _products;
        }

        public async Task<List<Product>?> GetPreviousProductsBySearch(string searchTerm)
        {
            var products = await SearchProducts(searchTerm);
            
            if (products.IsNullOrEmpty())
            {
                return null;
            }
            if (offset2 < 0)
            {
                offset2 = 0;
            }
            if (offset1 - offset2 > itemsPerPage)
            {
                offset1 = offset2 + itemsPerPage;
            }
            var _products = products.Skip(offset2).Take(itemsPerPage).ToList();
            offset2 -= itemsPerPage;
            return _products;
        }

        public async Task<List<Product>?> GoToPage(int page)
        {
            var count = await GetPageCount();
            if (page <= 0 || page > count)
            {
                return null;
            }
            offset1 = (page - 1) * itemsPerPage;
            offset2 = offset1 - itemsPerPage;
            return await GetNextProducts();
        }

        public async Task<List<Product>?> GoToPageByCategory(string category, int page)
        {
            var count = await GetPageCountByCategory(category);
            if (page <= 0 || page > count)
            {
                return null;
            }
            offset1 = (page - 1) * itemsPerPage;
            offset2 = offset1 - itemsPerPage;
            return await GetNextProductsByCategory(category);
        }

        public async Task<List<Product>?> GoToPageBySearch(string searchTerm, int page)
        {
            var count = await GetPageCountBySearch(searchTerm);
            if (page <= 0 || page > count)
            {
                return null;
            }
            offset1 = (page - 1) * itemsPerPage;
            offset2 = offset1 - itemsPerPage;
            return await GetNextProductsBySearch(searchTerm);
        }

        public async Task<int> GetPageCount()
        {
            var count = await CountRows();
            return (int)Math.Ceiling((double)count / itemsPerPage);
        }

        public async Task<int> GetPageCountByCategory(string category)
        {
            var count = await CountRowsByCategory(category);
            return (int)Math.Ceiling((double)count / itemsPerPage);
        }

        public async Task<int> GetPageCountBySearch(string searchTerm)
        {
            var products = await SearchProducts(searchTerm);
            var count = products.Count;
            return (int)Math.Ceiling((double)count / itemsPerPage);
        }
    }
}