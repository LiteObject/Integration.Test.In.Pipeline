using Demo.Product.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.Product.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly AppDbContext _context;

        public ProductsController(ILogger<ProductsController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _context = appDbContext;
        }

        [HttpGet(Name = "GetProducts")]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($">>> Invoked {nameof(Get)}");
            Domain.Product[] products = await _context.Products.ToArrayAsync();
            return Ok(products);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProductsAsync()
        {
            _logger.LogInformation($">>> Invoked {nameof(GetProductsAsync)}");

            Domain.Product[] products = await _context.Products.ToArrayAsync();
            return Ok(products);
        }
    }
}