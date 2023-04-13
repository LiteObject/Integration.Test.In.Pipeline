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
        public async Task<IActionResult> GetAsync()
        {
            _logger.LogInformation($">>> Invoked {nameof(GetAsync)}");
            Domain.Product[] products = await _context.Products.ToArrayAsync();
            return Ok(products);
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<IActionResult> GetAsync(int id)
        {
            _logger.LogInformation($">>> Invoked {nameof(GetAsync)}");

            Domain.Product? product = await _context.Products.FindAsync(id);

            return product is null ? NotFound() : Ok(product);
        }
    }
}