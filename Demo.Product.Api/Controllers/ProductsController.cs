using AutoMapper;
using AutoMapper.QueryableExtensions;
using Demo.Product.Api.Data;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IMapper _mapper;

        public ProductsController(ILogger<ProductsController> logger, AppDbContext appDbContext, IMapper mapper)
        {
            _logger = logger;
            _context = appDbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            _logger.LogInformation($">>> Invoked {nameof(GetAsync)}");
            DTO.Product[] productDtos = await _context.Products.ProjectTo<DTO.Product>(_mapper.ConfigurationProvider).ToArrayAsync();
            // DTO.Product[] productDtos = _mapper.Map<DTO.Product[]>(products);

            return Ok(productDtos);
        }

        [HttpGet("{id}")]
        [ActionName("GetByIdAsync")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            _logger.LogInformation($">>> Invoked {nameof(GetAsync)}");
            Domain.Product? product = await _context.Products.FindAsync(id);

            DTO.Product productDto = _mapper.Map<DTO.Product>(product);
            return productDto is null ? NotFound() : Ok(productDto);
        }

        [Authorize(Policy = "product-write-policy")]
        [HttpPost]
        public async Task<IActionResult> PostAsync(DTO.Product product)
        {
            Domain.Product newProduct = _mapper.Map<Domain.Product>(product);

            _ = await _context.Products.AddAsync(newProduct);
            _ = await _context.SaveChangesAsync();
            return Created($"/Products/{product?.Id}", product);
        }
    }
}