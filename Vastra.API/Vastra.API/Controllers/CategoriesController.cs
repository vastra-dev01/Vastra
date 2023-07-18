using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Vastra.API.Entities;
using Vastra.API.Models;
using Vastra.API.Services;

namespace Vastra.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IVastraRepository _vastraRepository;
        private readonly IMapper _mapper;
        const int maxCategoriesPageSize = 10;
        public CategoriesController(IVastraRepository vastraRepository, IMapper mapper)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > maxCategoriesPageSize)
            {
                pageSize = maxCategoriesPageSize;
            }
            var (categoryEntities, paginationMetadata) = await _vastraRepository.GetCategoriesAsync(pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categoryEntities));
        }
        
    }
}
