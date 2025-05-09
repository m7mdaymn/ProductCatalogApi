using Microsoft.AspNetCore.Mvc;
using ProductCatalogApi.Models;

namespace ProductCatalogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private static readonly List<Product> _products = new List<Product>
        {
            new Product { ProductId = 1, Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, ImageUrl = "https://example.com/laptop.jpg", Category = "Electronics", Quantity = 50 },
            new Product { ProductId = 2, Name = "Phone", Description = "Smartphone with 5G", Price = 599.99m, ImageUrl = "https://example.com/phone.jpg", Category = "Electronics", Quantity = 75 },
            new Product { ProductId = 3, Name = "Headphones", Description = "Noise-canceling headphones", Price = 149.99m, ImageUrl = "https://example.com/headphones.jpg", Category = "Audio", Quantity = 100 },
            new Product { ProductId = 4, Name = "Smartwatch", Description = "Fitness tracking smartwatch", Price = 199.99m, ImageUrl = "https://example.com/smartwatch.jpg", Category = "Wearables", Quantity = 30 },
            new Product { ProductId = 5, Name = "Tablet", Description = "10-inch tablet with stylus", Price = 399.99m, ImageUrl = "https://example.com/tablet.jpg", Category = "Electronics", Quantity = 60 }
        };

        private static readonly object _lock = new object(); // Thread safety

        [HttpPost]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            if (product == null || string.IsNullOrEmpty(product.Name) || product.Price <= 0)
            {
                return BadRequest(new { message = "Invalid product data. Name cannot be empty and Price must be positive." });
            }

            lock (_lock)
            {
                product.ProductId = _products.Count > 0 ? _products.Max(p => p.ProductId) + 1 : 1;
                _products.Add(product);
            }

            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }

        [HttpGet]
        public IActionResult GetAllProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? category = null)
        {
            var query = _products;

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var paginatedProducts = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                Data = paginatedProducts
            };

            return Ok(response);
        }

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

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product product)
        {
            if (product == null || string.IsNullOrEmpty(product.Name) || product.Price <= 0)
            {
                return BadRequest(new { message = "Invalid product data. Name cannot be empty and Price must be positive." });
            }

            lock (_lock)
            {
                var existingProduct = _products.FirstOrDefault(p => p.ProductId == id);
                if (existingProduct == null)
                {
                    return NotFound(new { message = $"Product with ID {id} not found." });
                }

                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.Category = product.Category;
                existingProduct.Quantity = product.Quantity;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            lock (_lock)
            {
                var product = _products.FirstOrDefault(p => p.ProductId == id);
                if (product == null)
                {
                    return NotFound(new { message = $"Product with ID {id} not found." });
                }

                _products.Remove(product);
            }

            return NoContent();
        }
    }
}