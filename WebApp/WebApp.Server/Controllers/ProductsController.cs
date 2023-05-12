using Microsoft.AspNetCore.Mvc;
using WebApp.Shared.Models;
using WebApp.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using System.Reflection.Metadata.Ecma335;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> Get()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("ID/{id}")]
        public async Task<ActionResult> GetByID(int id) => await _productService.IsValidID(id) ? Ok(await _productService.GetByIDAsync(id)) : NotFound("Product Not Found");

        [HttpGet("next")]
        public async Task<ActionResult<List<Product>>> GetNext()
        {
            var products = await _productService.GetNextProducts();
            if (products == null)
            {
                return NotFound("Not Found");
            }
            return Ok(products);
        }

        [HttpGet("previous")]
        public async Task<ActionResult<List<Product>>> GetPrevious()
        {
            var products = await _productService.GetPreviousProducts();
            if (products == null)
            {
                return NotFound("Not Found");
            }
            return Ok(products);
        }

        [HttpGet("[action]/{category}")]
        public async Task<ActionResult<List<Product>>> ByCategory(string category) => await _productService.IsExistingCategory(category) ? Ok(await _productService.GetByCategoryAsync(category)) : NotFound("Not Found");

        [HttpGet("{category}/next")]
        public async Task<ActionResult<List<Product>>> GetNextByCategory(string category)
        {
            if(await _productService.IsExistingCategory(category))
            {
                var products = await _productService.GetNextProductsByCategory(category);
                if(products != null)
                {
                    return Ok(products);
                }
            }
            return NotFound("Not Found");
        } 

        [HttpGet("{category}/previous")]
        public async Task<ActionResult<List<Product>>> GetPreviousByCategory(string category)
        {
            if (await _productService.IsExistingCategory(category))
            {
                var products = await _productService.GetPreviousProductsByCategory(category);
                if (products != null)
                {
                    return Ok(products);
                }
            }
            return NotFound("Not Found");
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<Product>>> GetCategories()
        {
            var categories = await _productService.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("page/{page}")]
        public async Task<ActionResult<List<Product>>> GoToPage(int page)
        {
            var products = await _productService.GoToPage(page);
            if (products == null)
            {
                return NotFound("Not Found");
            }
            return Ok(products);
        }

        [HttpGet("{category}/{page}")]
        public async Task<ActionResult<List<Product>>> GoToPageByCategory(string category, int page)
        {
            var products = await _productService.GoToPageByCategory(category, page);
            if (products == null)
            {
                return NotFound("Not Found");
            }
            return Ok(products);
        }

        [HttpGet("search/{page}")]
        public async Task<ActionResult<List<Product>>> GoToPageBySearch(string search, int page)
        {
            var products = await _productService.GoToPageBySearch(search, page);
            if (products == null)
            {
                return NotFound("Not Found");
            }
            return Ok(products);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Product>>> Search(string search)
        {
            var products = await _productService.SearchProducts(search);
            if (products.IsNullOrEmpty())
            {
                return NotFound("Product Not Found");
            }
            return Ok(products);    
        }

        [HttpGet("search/next")]
        public async Task<ActionResult<List<Product>>> GetNextBySearch(string search)
        {
            var products = await _productService.GetNextProductsBySearch(search);
            if (products == null)
            {
                return NotFound("Product Not Found");
            }
            return Ok(products);
        }

        [HttpGet("search/previous")]
        public async Task<ActionResult<List<Product>>> GetPreviousBySearch(string search)
        {
            var products = await _productService.GetPreviousProductsBySearch(search);
            if (products == null)
            {
                return NotFound("Product Not Found");
            }
            return Ok(products);
        }

        [HttpGet("PageCount")]
        public async Task<ActionResult<List<Product>>> GetPageCount()
        {
            var count = await _productService.GetPageCount();
            return Ok(count);
        }

        [HttpGet("{category}/PageCount")]
        public async Task<ActionResult<List<Product>>> GetPageCountByCategory(string category)
        {
            var count = await _productService.GetPageCountByCategory(category);
            return Ok(count);
        }

        [HttpGet("search/PageCount")]
        public async Task<ActionResult<List<Product>>> GetPageCountBySearch(string search)
        {
            var count = await _productService.GetPageCountBySearch(search);
            return Ok(count);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Insert(Product product)
        {
            /*if (product == null)
            {
                return BadRequest(new { message = "Request parameters are not correct" });
            }
            else
            {*/
                await _productService.InsertAsync(product);
                return Ok(new { message = "New product inserted successfully" });
           // }
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Update(Product product)
        {
            /*if (product == null)
            {
                return BadRequest(new { message = "Request parameters are not correct" });
            }
            else*/ if (await _productService.IsValidID(product.ID))
            {
                await _productService.UpdateAsync(product);
                return Ok(new { message = $"Product with ID: {product.ID} updated successfully" });
            }
            return NotFound(new { message = "Product Not Found" });
        }
 
        [HttpDelete("[action]/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(int id)
        {
            if(await _productService.IsValidID(id))
            {
                await _productService.DeleteAsync(id);
                return Ok(new { message = $"Product with ID: {id} deleted successfully" });
            }
            return NotFound(new { message = "Product Not Found" });
        }
    }
}
