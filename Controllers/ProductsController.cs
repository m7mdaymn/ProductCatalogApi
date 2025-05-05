using Microsoft.AspNetCore.Mvc;
using ProductCatalogApi.Models;

namespace ProductCatalogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private static List<Product> _products = new List<Product>
        {
            new Product { ProductId = 1, Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, ImageUrl = "https://example.com/laptop.jpg" },
            new Product { ProductId = 2, Name = "Phone", Description = "Smartphone with 5G", Price = 599.99m, ImageUrl = "https://example.com/phone.jpg" },
            new Product { ProductId = 3, Name = "Headphones", Description = "Noise-canceling headphones", Price = 149.99m, ImageUrl = "https://example.com/headphones.jpg" },
            new Product { ProductId = 4, Name = "Smartwatch", Description = "Fitness tracking smartwatch", Price = 199.99m, ImageUrl = "https://example.com/smartwatch.jpg" },
            new Product { ProductId = 5, Name = "Tablet", Description = "10-inch tablet with stylus", Price = 399.99m, ImageUrl = "https://example.com/tablet.jpg" }
        };

        // POST: api/products
        [HttpPost]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            if (product == null || string.IsNullOrEmpty(product.Name) || product.Price <= 0)
            {
                return BadRequest(new { message = "Invalid product data. Name cannot be empty and Price must be positive." });
            }

            product.ProductId = _products.Count > 0 ? _products.Max(p => p.ProductId) + 1 : 1;
            _products.Add(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }

        // GET: api/products
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            return Ok(_products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }
            return Ok(product);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product product)
        {
            if (product == null || string.IsNullOrEmpty(product.Name) || product.Price <= 0)
            {
                return BadRequest(new { message = "Invalid product data. Name cannot be empty and Price must be positive." });
            }

            var existingProduct = _products.FirstOrDefault(p => p.ProductId == id);
            if (existingProduct == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.ImageUrl = product.ImageUrl;
            return NoContent();
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }

            _products.Remove(product);
            return NoContent();
        }
    }
}