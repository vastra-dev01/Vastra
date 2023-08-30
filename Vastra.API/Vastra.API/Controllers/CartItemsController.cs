using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Xml.XPath;
using Vastra.API.Entities;
using Vastra.API.Enums;
using Vastra.API.Models;
using Vastra.API.Models.CustomException;
using Vastra.API.Models.ForCreationAndUpdate;
using Vastra.API.Services;

namespace Vastra.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/roles/{roleId}/users/{userId}/orders/{orderId}/cartItems")]
    public class CartItemsController : Controller
    {
        private readonly IVastraRepository _vastraRepository;
        private readonly IMapper _mapper;
        private const int maxCartItemPageSize = 10;
        private readonly ILogger<CartItemsController> _logger;

        public CartItemsController(IVastraRepository vastraRepository, IMapper mapper, ILogger<CartItemsController> logger)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCartItems(int roleId, int userId, 
            int orderId, int pageNumber= 1, int pageSize = 10)
        {
            _logger.LogDebug("Inside GetCartItems in CartItemsController");
            if(!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug($"Role with roleId {roleId} was not found in CartItemsController.");
                return NotFound();
            }
            if(!await _vastraRepository.UserExistsAsync(userId))
            {
                _logger.LogDebug($"User with userId {userId} was not found in CartItemsController.");
                return NotFound();
            }
            if(!await _vastraRepository.OrderExistsForUser(userId, orderId))
            {
                _logger.LogDebug($"Order with orderId {orderId} and userId {userId} was not found " +
                    $"in CartItemsController");
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug($"User claim failed for userId {userId} in CartItemsController.");
                return Forbid();
            }
            if (pageSize > maxCartItemPageSize)
            {
                pageSize = maxCartItemPageSize;
            }
            var (cartItemEntities, paginationMetadata) = await _vastraRepository.GetCartItemsForOrderAsync(orderId,
                pageNumber, pageSize);
            
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            _logger.LogInformation($"Total {cartItemEntities.Count()} cartItems fetched " +
                $"for  orderId {orderId} in CartItemsController");

            return Ok(_mapper.Map<IEnumerable<CartItemDto>>(cartItemEntities));
        }
        [HttpHead("{cartItemId}")]
        [HttpGet("{cartItemId}", Name = "GetCartItem")]
        public async Task<ActionResult<CartItemDto>> GetCartItem(int roleId, int userId, int orderId,
            int cartItemId, bool includeProduct = false)
        {
            _logger.LogDebug("Inside GetCartItem in CartItemsController");
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug($"Role with roleId {roleId} was not found in CartItemsController.");
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsAsync(userId))
            {
                _logger.LogDebug($"User with userId {userId} was not found in CartItemsController.");
                return NotFound();
            }
            if (!await _vastraRepository.OrderExistsForUser(userId, orderId))
            {
                _logger.LogDebug($"Order with orderId {orderId} and userId {userId} was not found " +
                    $"in CartItemsController.");
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug($"User claim failed for userId {userId} in CartItemsController.");
                return Forbid();
            }
            var cartItem = await _vastraRepository.GetCartItemForOrderAsync(orderId, cartItemId, includeProduct);
            if(cartItem == null)
            {
                _logger.LogDebug($"CartItem with id {cartItemId} and orderId {orderId} was not found " +
                    $"in CartItemsController.");
                return NotFound();
            }
            if (includeProduct)
            {
                _logger.LogInformation($"Successfully returning cart item {cartItemId} " +
                    $"with productId {cartItem.ProductId} included.");
                return Ok(_mapper.Map<CartItemWithProductDto>(cartItem));
            }
            else
            {
                _logger.LogInformation($"Successfully returning cart item {cartItemId}.");
                return Ok(_mapper.Map<CartItemDto>(cartItem));
            }
        }
        [HttpPost]
        public async Task<ActionResult<CartItemDto>> CreateCartItem(int roleId, int userId, int orderId, 
            CartItemForCreationDto cartItem)
        {
            _logger.LogDebug("Inside CreateCartItem in CartItemsController");
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug($"Role with roleId {roleId} was not found in CartItemsController.");
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsAsync(userId))
            {
                _logger.LogDebug($"User with userId {userId} was not found in CartItemsController.");
                return NotFound();
            }
            if (!await _vastraRepository.OrderExistsForUser(userId, orderId))
            {
                _logger.LogDebug($"Order with orderId {orderId} and userId {userId} was not found " +
                    $"in CartItemsController.");
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug($"User claim failed for userId {userId} in CartItemsController.");
                return Forbid();
            }
            //if order has been placed and payment done, cart items can't be added to it
            var order = await _vastraRepository.GetOrderAsync(orderId);
            if (order.PaymentStatus.Equals(PaymentStatus.Success.ToString()))
            {
                _logger.LogDebug($"CartItem creation failed as payment has already been done " +
                    $"for orderId {orderId}.");
                throw new UpdatePaidOrderException("Can't add cart item to a paid order.");
            }
            //check if product with product id given in cart item exists
            var product = await _vastraRepository.GetProductAsync(cartItem.ProductId);
            if (product == null)
            {
                _logger.LogDebug($"Product with productId {cartItem.ProductId} was not found " +
                    $"in CartItemsController");
                return NotFound();
            }
            //check if product already exists as a cart item
            var existingCartItem = await _vastraRepository.ProductExistsAsACartItemForOrder(orderId,
                cartItem.ProductId);
            if (existingCartItem != null)
            {
                _logger.LogDebug($"ProductId {cartItem.ProductId} already exists " +
                    $"in orderId {order} as a cartItem. " +
                    $"Sending update request.");
                return await UpdateCartItem(roleId, userId, orderId, existingCartItem.CartItemId,
                new CartItemForUpdateDto
                {
                    Quantity = cartItem.Quantity
                });
                
            }
            //check if added quantity is available
            if(cartItem.Quantity > product.Quantity)
            {
                _logger.LogDebug($"Quantity({cartItem.Quantity}) " +
                    $"is greater than available quantity({product.Quantity}); ");
                throw new QuantityOutOfLimitException("Quantity is more than available quantity.");
            }
            var finalCartItem = _mapper.Map<Entities.CartItem>(cartItem);

            //set value of cart item
            finalCartItem.Value = product.Price * cartItem.Quantity;
            _logger.LogDebug($"finalCartItem.Value = {finalCartItem.Value} " +
                $"in CreateCartItem() " +
                $"in CartItemsController");

            //set date added and date modified for newly created cart item
            finalCartItem.DateAdded = DateTime.Now;
            finalCartItem.DateModified = DateTime.Now;

            _logger.LogDebug($"finalCartItem.DateAdded = {finalCartItem.DateAdded} & " +
                $"finalCartItem.DateModified = {finalCartItem.DateModified} " +
                $"in CreateCartItem() " +
                $"in CartItemsController.");

            //add cart item to order
            await _vastraRepository.AddCartItemForOrderAsync(orderId, finalCartItem);

            //update order value
            var oldOrderValue = order.Value;
            order.Value = oldOrderValue + finalCartItem.Value;
            _logger.LogDebug($"Updated order value order.Value = {order.Value} in CreateCartItem() " +
                $"in CartItemsController.");
            //save changes to db
            await _vastraRepository.SaveChangesAsync();

            var createdCartItemToReturn = _mapper.Map<CartItemDto>(finalCartItem);
            _logger.LogInformation($"Successfully returning created cart item :" +
                $" {JsonSerializer.Serialize(createdCartItemToReturn)}");
            return CreatedAtRoute("GetCartItem",
                new
                {
                    roleId = roleId,
                    userId = userId,
                    orderId = orderId,
                    cartItemId = createdCartItemToReturn.CartItemId
                },
                createdCartItemToReturn

                );
        }
        [HttpPut("cartItemId")]
        public async Task<ActionResult> UpdateCartItem(int roleId, int userId, int orderId, int cartItemId,
            CartItemForUpdateDto cartItem)
        {
            _logger.LogDebug("Inside UpdateCartItem in CartItemsController");
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug($"Role with roleId {roleId} was not found in CartItemsController.");
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsAsync(userId))
            {
                _logger.LogDebug($"User with userId {userId} was not found in CartItemsController.");
                return NotFound();
            }
            if (!await _vastraRepository.OrderExistsForUser(userId, orderId))
            {
                _logger.LogDebug($"Order with orderId {orderId} and userId {userId} was not found " +
                    $"in CartItemsController.");
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug($"User claim failed for userId {userId} in CartItemsController.");
                return Forbid();
            }
            //if order has been placed and payment done, cart items can't be modified
            var order = await _vastraRepository.GetOrderAsync(orderId);
            if (order.PaymentStatus.Equals(PaymentStatus.Success.ToString()))
            {
                _logger.LogDebug($"CartItem updation failed as payment has already been done " +
                    $"for orderId {orderId}.");
                throw new UpdatePaidOrderException("Can't update cart item of a paid order.");
            }
            var cartItemEntity = await _vastraRepository.GetCartItemForOrderAsync(orderId, cartItemId);
            if (cartItemEntity == null)
            {
                _logger.LogDebug($"Cart Item with cartItemId {cartItemId} and orderId {orderId}" +
                    $" was not found in CartItemsController.");
                return NotFound(cartItemId);
            }
            //check if modified quantity is available
            var product = await _vastraRepository.GetProductAsync(cartItemEntity.ProductId);
            if (product == null)
            {
                _logger.LogDebug($"Product with productId {cartItemEntity.ProductId}" +
                    $" was not found in CartItemsController");
                return NotFound();
            }
            if (cartItem.Quantity > product.Quantity)
            {
                _logger.LogDebug($"Quantity({cartItem.Quantity}) " +
                    $"is greater than available quantity({product.Quantity}); ");
                throw new QuantityOutOfLimitException("Quantity is more than available quantity.");
            }
            
            float oldCartItemValue = cartItemEntity.Value;

            _logger.LogDebug($"oldCartItemvalue = {oldCartItemValue} " +
                $"in UpdateCartItem() " +
                $"in CartItemsController");
            _mapper.Map(cartItem, cartItemEntity);

            //update Value of cart item
            cartItemEntity.Value = cartItem.Quantity * product.Price;
            _logger.LogDebug($"Updated cartItemEntity.Value = {cartItemEntity.Value} " +
                $"in UpdateCartItem() " +
                $"in CartItemsController");
            //update Modified Time of cartItem
            cartItemEntity.DateModified = DateTime.Now;
            _logger.LogDebug($"Updated cartItemEntity.DateModified = {cartItemEntity.DateModified} " +
                $"in UpdateCartItem() " +
                $"in CartItemsController");
            //update order value
            var oldOrderValue = order.Value;
            order.Value = oldOrderValue - oldCartItemValue + cartItemEntity.Value;
            _logger.LogDebug($"Updated order.Value = {order.Value} " +
               $"in UpdateCartItem() " +
               $"in CartItemsController");

            await _vastraRepository.SaveChangesAsync();
            _logger.LogInformation($"Cart Item with cartItemId {cartItemId} updated.");
            return NoContent();
        }
        [HttpPatch("{cartItemId}")]
        public async Task<ActionResult> PartiallyUpdateCartItem(int roleId, int userId, int orderId,
            int cartItemId,
            JsonPatchDocument<CartItemForUpdateDto> patchDocument)
        {
            _logger.LogDebug("Inside PartiallyUpdateCartItem in CartItemsController");
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug($"Role with roleId {roleId} was not found in CartItemsController.");
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsAsync(userId))
            {
                _logger.LogDebug($"User with userId {userId} was not found in CartItemsController.");
                return NotFound();
            }
            if (!await _vastraRepository.OrderExistsForUser(userId, orderId))
            {
                _logger.LogDebug($"Order with orderId {orderId} and userId {userId} was not found " +
                    $"in CartItemsController.");
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug($"User claim failed for userId {userId} in CartItemsController.");
                return Forbid();
            }
            //if order has been placed and payment done, cart items can't be modified
            var order = await _vastraRepository.GetOrderAsync(orderId);
            if (order.PaymentStatus.Equals(PaymentStatus.Success.ToString()))
            {
                _logger.LogDebug($"CartItem updation failed as payment has already been done " +
                    $"for orderId {orderId}.");
                throw new UpdatePaidOrderException("Can't update cart item of a paid order.");
            }
            var cartItemEntity = await _vastraRepository.GetCartItemForOrderAsync(orderId, cartItemId);
            if (cartItemEntity == null)
            {
                _logger.LogDebug($"Cart Item with cartItemId {cartItemId} and orderId {orderId}" +
                    $" was not found in CartItemsController.");
                return NotFound(cartItemId);
            }
            //check if modified quantity is available
            var product = await _vastraRepository.GetProductAsync(cartItemEntity.ProductId);
            if (product == null)
            {
                _logger.LogDebug($"Product with productId {cartItemEntity.ProductId}" +
                    $" was not found in CartItemsController");
                return NotFound();
            }
            var cartItemToPatch = _mapper.Map<CartItemForUpdateDto>(cartItemEntity);
            if (cartItemToPatch.Quantity > product.Quantity)
            {
                _logger.LogDebug($"Quantity({cartItemToPatch.Quantity}) " +
                    $"is greater than available quantity({product.Quantity}); ");
                throw new QuantityOutOfLimitException("Quantity is more than available quantity.");
            }

            float oldCartItemValue = cartItemEntity.Value;
            _logger.LogDebug($"oldCartItemvalue = {oldCartItemValue} " +
                $"in PartiallyUpdateCartItem() " +
                $"in CartItemsController");
            patchDocument.ApplyTo(cartItemToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                _logger.LogDebug($"Validation failed for patchDocument in CartItemsController.");
                return BadRequest(ModelState);
            }
            if (!TryValidateModel(cartItemToPatch))
            {
                _logger.LogDebug($"Validation failed for patchDocument in CartItemsController.");
                return BadRequest(ModelState);
            }
            _mapper.Map(cartItemToPatch, cartItemEntity);

            //update value of cart item
            cartItemEntity.Value = cartItemToPatch.Quantity * product.Price;
            _logger.LogDebug($"Updated cartItemEntity.Value = {cartItemEntity.Value} " +
                $"in PartiallyUpdateCartItem() " +
                $"in CartItemsController");
            //update Modified Time of cartItem
            cartItemEntity.DateModified = DateTime.Now;
            _logger.LogDebug($"Updated cartItemEntity.DateModified = {cartItemEntity.DateModified} " +
                $"in PartiallyUpdateCartItem() " +
                $"in CartItemsController");
            //update order value
            var oldOrderValue = order.Value;
            order.Value = oldOrderValue - oldCartItemValue + cartItemEntity.Value;
            _logger.LogDebug($"Updated order.Value = {order.Value} " +
               $"in PartiallyUpdateCartItem() " +
               $"in CartItemsController");
            await _vastraRepository.SaveChangesAsync();

            _logger.LogInformation($"Cart Item with cartItemId {cartItemId} updated.");

            return NoContent();
        }
        [HttpDelete("{cartItemId}")]
        public async Task<ActionResult> DeleteCartItem(int roleId, int userId, int orderId, int cartItemId)
        {
            _logger.LogDebug("Inside DeleteCartItem in CartItemsController");
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug($"Role with roleId {roleId} was not found in CartItemsController.");
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsAsync(userId))
            {
                _logger.LogDebug($"User with userId {userId} was not found in CartItemsController.");
                return NotFound();
            }
            if (!await _vastraRepository.OrderExistsForUser(userId, orderId))
            {
                _logger.LogDebug($"Order with orderId {orderId} and userId {userId} was not found " +
                    $"in CartItemsController.");
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug($"User claim failed for userId {userId} in CartItemsController.");
                return Forbid();
            }
            //if order has been placed and payment done, cart items can't be modified
            var order = await _vastraRepository.GetOrderAsync(orderId);
            if (order.PaymentStatus.Equals(PaymentStatus.Success.ToString()))
            {
                _logger.LogDebug($"CartItem deletion failed as payment has already been done " +
                    $"for orderId {orderId}.");
                throw new UpdatePaidOrderException("Can't delete a paid order.");
            }
            var cartItemToBeDeleted = await _vastraRepository.GetCartItemForOrderAsync(orderId, cartItemId);
            if (cartItemToBeDeleted == null)
            {
                _logger.LogDebug($"Cart Item with id {cartItemId} was not found in CartItemsController.");
                return NotFound(cartItemId);
            }
            _vastraRepository.DeleteCartItem(cartItemToBeDeleted);

            //update order value
            var oldOrderValue = order.Value;
            order.Value = oldOrderValue - cartItemToBeDeleted.Value;
            _logger.LogDebug($"Updated order.Value = {order.Value} " +
               $"in DeleteCartItem() " +
               $"in CartItemsController");

            await _vastraRepository.SaveChangesAsync();
            _logger.LogInformation($"Cart Item with id {cartItemId} was deleted successfully");
            return NoContent();
        }

    }
}
