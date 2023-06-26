using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Vastra.API.Models;
using Vastra.API.Services;

namespace Vastra.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IVastraRepository _vastraRepository;
        private readonly IMapper _mapper;
        const int maxProductsPageSize = 20;
        public ProductsController(IVastraRepository vastraRepository, IMapper mapper)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(
            string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > maxProductsPageSize)
            {
                pageSize = maxProductsPageSize;
            }
            var (productEntities, paginationMetadata) = await _vastraRepository.GetProductsAsync(
                name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            return Ok(_mapper.Map<IEnumerable<ProductDto>>(productEntities));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult>GetProduct(int id)
        {
            var product = await _vastraRepository.GetProductAsync(id);
            if(product == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ProductDto>(product));
        }
    }
}
