using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Vastra.API.DBContexts;
using Vastra.API.Entities;
using Vastra.API.Models;
using Vastra.API.Models.ForCreationAndUpdate;
using Vastra.API.Services;

namespace Vastra.API.Controllers
{
    [Route("api/roles/{roleId}/users/{userId}/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IVastraRepository _vastraRepository;
        private readonly IMapper _mapper;
        const int maxOrdersPageSize = 20;
        public OrdersController(IVastraRepository vastraRepository, IMapper mapper)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(int roleId, int userId, int pageNumber = 1, int pageSize = 10)
        {
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            { 
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsWithRoleAsync(roleId, userId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                return Forbid();
            }
            if (pageSize > maxOrdersPageSize)
            {
                pageSize = maxOrdersPageSize;
            }
            var (orderEntities, paginationMetadata) = await _vastraRepository.GetOrdersForUserAsync(userId, pageSize, pageNumber);
                
            return Ok(_mapper.Map<IEnumerable<OrderWithoutCartItemsDto>>(orderEntities));
        }


        [HttpHead("{orderId}")]
        [HttpGet("{orderId}", Name = "GetOrder")]
        [Authorize]
        public async Task<IActionResult> GetOrder(int roleId, int userId, int orderId, bool includeCartItems = false )
        {
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsWithRoleAsync(roleId, userId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                return Forbid();
            }
            var order = await _vastraRepository.GetOrderForUserAsync(userId, orderId, includeCartItems);
            if (order == null)
            {
                return NotFound();
            }
            if (includeCartItems)
            {
                return Ok(_mapper.Map<OrderDto>(order));
            }
            else
            {
                return Ok(_mapper.Map<OrderWithoutCartItemsDto>(order));
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderDto>> CreateOrder(int roleId, int userId, OrderForCreationDto order)
        {
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsWithRoleAsync(roleId, userId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                return Forbid();
            }
            var finalorder = _mapper.Map<Entities.Order>(order);

            finalorder.DateAdded = DateTime.Now;
            finalorder.DateModified = DateTime.Now;
            await _vastraRepository.AddOrderForUserAsync(userId, finalorder);
            await _vastraRepository.SaveChangesAsync();

            var createdOrderToReturn = _mapper.Map<OrderWithoutCartItemsDto>(finalorder);
            return CreatedAtRoute("GetOrder",
                new
                {
                    roleId = roleId,
                    userId = userId,
                    orderId = createdOrderToReturn.OrderId
                },
                createdOrderToReturn

                );
        }
/*
        [HttpPut]
        public async Task<ActionResult> UpdateOrder(int roleId, int userId, int orderId,
            OrderForUpdateDto order)
        {
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                return NotFound();
            }
            if(!await _vastraRepository.UserExistsAsync(userId))
            { 
                return NotFound(); 
            }
            var orderEntity = await _vastraRepository.GetOrderForUserAsync(userId, orderId);
            if (orderEntity == null)
            {
                return NotFound();
            }
            _mapper.Map(order, orderEntity);
            //update Modified Time of product
            orderEntity.DateModified = DateTime.Now;
            await _vastraRepository.SaveChangesAsync();
            return NoContent();

        }
*/

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteOrder(int roleId, int userId, int orderId)
        {
            if(!await _vastraRepository.RoleExistsAsync(roleId))
            {
                return NotFound();
            }
            if(!await _vastraRepository.UserExistsWithRoleAsync(roleId, userId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                return Forbid();
            }
            var orderToDelete = await _vastraRepository.GetOrderForUserAsync(userId, orderId);
            if(orderToDelete == null)
            {
                return NotFound();
            }
            _vastraRepository.DeleteOrder(orderToDelete);
            await _vastraRepository.SaveChangesAsync();
            return NoContent();
        }


    }
}

