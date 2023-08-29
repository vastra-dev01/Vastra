using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Xml.Linq;
using Vastra.API.DBContexts;
using Vastra.API.Entities;
using Vastra.API.Enums;
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
        private readonly ILogger<OrdersController> _logger;
        const int maxOrdersPageSize = 20;

        public OrdersController(IVastraRepository vastraRepository, IMapper mapper, ILogger<OrdersController> logger)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(int roleId, int userId, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogDebug($"Inside GetOrders in OrdersController.");
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug($"Role with id {roleId} was not found " +
                    $"in GetOrders() " +
                    $"in OrdersController.");
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsWithRoleAsync(roleId, userId))
            {
                _logger.LogDebug($"User with id {userId} & roleId {roleId} was not found " +
                    $"in GetOrders() " +
                    $"in OrdersController.");
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug($"User claim failed for userId {userId} " +
                    $"in GetOrders() " +
                    $"in OrdersController.");
                return Forbid();
            }
            if (pageSize > maxOrdersPageSize)
            {
                pageSize = maxOrdersPageSize;
            }
            var (orderEntities, paginationMetadata) = await _vastraRepository.GetOrdersForUserAsync(userId, pageSize, pageNumber);

            _logger.LogInformation($"Total {orderEntities.Count()} orders fetched for userId {userId} " +
                $"in OrdersController.");

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<OrderWithoutCartItemsDto>>(orderEntities));
        }


        [HttpHead("{orderId}")]
        [HttpGet("{orderId}", Name = "GetOrder")]
        [Authorize]
        public async Task<IActionResult> GetOrder(int roleId, int userId, int orderId, bool includeCartItems = false )
        {
            _logger.LogDebug($"Inside GetOrder in OrdersController.");
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug($"Role with id {roleId} was not found " +
                    $"in GetOrder() " +
                    $"in OrdersController.");
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsWithRoleAsync(roleId, userId))
            {
                _logger.LogDebug($"User with id {userId} & roleId {roleId} was not found " +
                    $"in GetOrder() " +
                    $"in OrdersController.");
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug($"User claim failed for userId {userId} " +
                    $"in GetOrder() " +
                    $"in OrdersController.");
                return Forbid();
            }
            var order = await _vastraRepository.GetOrderForUserAsync(userId, orderId, includeCartItems);
            if (order == null)
            {
                _logger.LogDebug($"Order with id {orderId} was not found " +
                    $"in GetOrder() " +
                    $"in OrdersController.");
                return NotFound();
            }
            if (includeCartItems)
            {
                _logger.LogInformation($"Successfully returning order with id {order.OrderId} " +
                    $"including CartItems " +
                    $"in OrdersController.");
                return Ok(_mapper.Map<OrderDto>(order));
            }
            else
            {
                _logger.LogInformation($"Successfully returning order with id {order.OrderId} " +
                    $"in OrdersController.");
                return Ok(_mapper.Map<OrderWithoutCartItemsDto>(order));
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderDto>> CreateOrder(int roleId, int userId, OrderForCreationDto order)
        {
            _logger.LogDebug($"Inside CreateOrder in OrdersController.");
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug($"Role with id {roleId} was not found " +
                    $"in CreateOrder() " +
                    $"in OrdersController.");
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsWithRoleAsync(roleId, userId))
            {
                _logger.LogDebug($"User with id {userId} & roleId {roleId} was not found " +
                    $"in CreateOrder() " +
                    $"in OrdersController.");
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug($"User claim failed for userId {userId} " +
                    $"in CreateOrder() " +
                    $"in OrdersController.");
                return Forbid();
            }
            var finalorder = _mapper.Map<Entities.Order>(order);

            //set date added and date modified for newly created order
            finalorder.DateAdded = DateTime.Now;
            finalorder.DateModified = DateTime.Now;

            _logger.LogDebug($"Updated finalorder.DateAdded = {finalorder.DateAdded} " +
                $"& finalorder.DateModified = {finalorder.DateModified} " +
                $"in CreateOrder() " +
                $"in OrdersController.");

            //set payment status as pending
            finalorder.PaymentStatus = PaymentStatus.Pending.ToString();
            _logger.LogDebug($"Updated finalorder.PaymentStatus = {finalorder.PaymentStatus} " +
                $"in CreateOrder() " +
                $"in OrdersController.");
            //set initial value as 0
            finalorder.Value = 0;
            _logger.LogDebug($"Updated finalorder.Value = {finalorder.Value} " +
                $"in CreateOrder() " +
                $"in OrdersController.");
            //create order for user
            await _vastraRepository.AddOrderForUserAsync(userId, finalorder);
            await _vastraRepository.SaveChangesAsync();

            var createdOrderToReturn = _mapper.Map<OrderWithoutCartItemsDto>(finalorder);

            _logger.LogInformation($"Successfully returning created order with id " +
                $"{createdOrderToReturn.OrderId} " +
                $"in OrdersController.");
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

        //[HttpPut("{orderId}")]
        //public async Task<ActionResult> UpdateOrder(int roleId, int userId, int orderId,
        //    OrderForUpdateDto order)
        //{
        //    if (!await _vastraRepository.RoleExistsAsync(roleId))
        //    {
        //        return NotFound();
        //    }
        //    if(!await _vastraRepository.UserExistsAsync(userId))
        //    { 
        //        return NotFound(); 
        //    }
        //    var orderEntity = await _vastraRepository.GetOrderForUserAsync(userId, orderId);
        //    if (orderEntity == null)
        //    {
        //        return NotFound();
        //    }
        //    _mapper.Map(order, orderEntity);
        //    //update Modified Time of product
        //    orderEntity.DateModified = DateTime.Now;
        //    await _vastraRepository.SaveChangesAsync();
        //    return NoContent();

        //}


        [HttpDelete("{orderId}")]
        [Authorize]
        public async Task<ActionResult> DeleteOrder(int roleId, int userId, int orderId)
        {
            _logger.LogDebug($"Inside DeleteOrder in OrdersController.");
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug($"Role with id {roleId} was not found " +
                    $"in DeleteOrder() " +
                    $"in OrdersController.");
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsWithRoleAsync(roleId, userId))
            {
                _logger.LogDebug($"User with id {userId} & roleId {roleId} was not found " +
                    $"in DeleteOrder() " +
                    $"in OrdersController.");
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug($"User claim failed for userId {userId} " +
                    $"in DeleteOrder() " +
                    $"in OrdersController.");
                return Forbid();
            }
            var orderToDelete = await _vastraRepository.GetOrderForUserAsync(userId, orderId);
            if(orderToDelete == null)
            {
                _logger.LogDebug($"Order with id {orderId} was not found " +
                    $"in DeleteOrder() " +
                    $"in OrdersController.");
                return NotFound();
            }
            //if order has been placed and payment done, order can't be deleted
            if (orderToDelete.PaymentStatus.Equals(PaymentStatus.Success.ToString()))
            {
                _logger.LogDebug($"Order with id {orderId} can not be deleted as " +
                    $"payment has been done.");
                return BadRequest();
            }
            _vastraRepository.DeleteOrder(orderToDelete);
            await _vastraRepository.SaveChangesAsync();
            _logger.LogInformation($"Order with id {orderId} deleted successfully.");
            return NoContent();
        }


    }
}

