using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Vastra.API.Entities;
using Vastra.API.Models;
using Vastra.API.Services;

namespace Vastra.API.Controllers
{
    [ApiController]
    [Route("api/adminUtilities")]
    [Authorize(Policy = "MustBeAdmin")]
    public class AdminUtilitiesController : ControllerBase
    {
        private readonly IVastraRepository _vastraRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminUtilitiesController> _logger;
        const int maxPageSize = 20;
        public AdminUtilitiesController(IVastraRepository vastraRepository,
            IMapper mapper, ILogger<AdminUtilitiesController> logger)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<OrderWithoutCartItemsDto>>> GetOrders(
            int pageNumber = 1, int pageSize = 20)
        {
            _logger.LogDebug("Inside GetOrders in AdminUtilitiesController.");
            if (pageSize > maxPageSize)
            {
                pageSize = maxPageSize;
            }
            var (orderEntities, paginationMetadata) = await _vastraRepository
                .GetOrdersAsync(pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            _logger.LogInformation($"Total {orderEntities.Count()} orders fetched in AdminUtilitiesController.");

            return Ok(_mapper.Map<IEnumerable<OrderWithoutCartItemsDto>>(orderEntities));

        }
    }
}
