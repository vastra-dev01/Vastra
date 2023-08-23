using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Xml.XPath;
using Vastra.API.Entities;
using Vastra.API.Enums;
using Vastra.API.Models;
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
        public CartItemsController(IVastraRepository vastraRepository, IMapper mapper)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCartItems(int roleId, int userId, int orderId, int pageNumber= 1, int pageSize = 10)
        {
            if(!await _vastraRepository.RoleExistsAsync(roleId))
            {
                return NotFound();
            }
            if(!await _vastraRepository.UserExistsAsync(userId))
            {
                return NotFound();
            }
            if(!await _vastraRepository.OrderExistsForUser(userId, orderId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                return Forbid();
            }
            if (pageSize > maxCartItemPageSize)
            {
                pageSize = maxCartItemPageSize;
            }
            var (cartItemEntities, paginationMetadata) = await _vastraRepository.GetCartItemsForOrderAsync(orderId, pageNumber, pageSize);
            
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<CartItemDto>>(cartItemEntities));
        }
        [HttpHead("{cartItemId}")]
        [HttpGet("{cartItemId}", Name = "GetCartItem")]
        public async Task<ActionResult<CartItemDto>> GetCartItem(int roleId, int userId, int orderId, int cartItemId, bool includeProduct = false)
        {
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsAsync(userId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.OrderExistsForUser(userId, orderId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                return Forbid();
            }
            var cartItem = await _vastraRepository.GetCartItemForOrderAsync(orderId, cartItemId, includeProduct);
            if (includeProduct)
            {
                return Ok(_mapper.Map<CartItemWithProductDto>(cartItem));
            }
            else
            {
                return Ok(_mapper.Map<CartItemDto>(cartItem));
            }
        }
        [HttpPost]
        public async Task<ActionResult<CartItemDto>> CreateCartItem(int roleId, int userId, int orderId, CartItemForCreationDto cartItem)
        {
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsAsync(userId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.OrderExistsForUser(userId, orderId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                return Forbid();
            }
            //if order has been placed and payment done, cart items can't be added to it
            var order = await _vastraRepository.GetOrderAsync(orderId);
            if (order.PaymentStatus.Equals(PaymentStatus.Success.ToString()))
            {
                return BadRequest();
            }
            //check if product with product id given in cart item exists
            var product = await _vastraRepository.GetProductAsync(cartItem.ProductId);
            if (product == null)
            {
                return NotFound();
            }
            //check if product already exists as a cart item
            var existingCartItem = await _vastraRepository.ProductExistsAsACartItemForOrder(orderId, cartItem.ProductId);
            if (existingCartItem != null)
            {
                return await UpdateCartItem(roleId, userId, orderId, existingCartItem.CartItemId,
                new CartItemForUpdateDto
                {
                    Quantity = cartItem.Quantity
                });
                
            }
            //check if added quantity is available
            if(cartItem.Quantity > product.Quantity)
            {
                return BadRequest();
            }
            var finalCartItem = _mapper.Map<Entities.CartItem>(cartItem);

            //set value of cart item
            finalCartItem.Value = product.Price * cartItem.Quantity;

            //set date added and date modified for newly created cart item
            finalCartItem.DateAdded = DateTime.Now;
            finalCartItem.DateModified = DateTime.Now;

            //add cart item to order
            await _vastraRepository.AddCartItemForOrderAsync(orderId, finalCartItem);

            //update order value
            var oldOrderValue = order.Value;
            order.Value = oldOrderValue + finalCartItem.Value;

            //save changes to db
            await _vastraRepository.SaveChangesAsync();

            var createdCartItemToReturn = _mapper.Map<CartItemDto>(finalCartItem);
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
        public async Task<ActionResult> UpdateCartItem(int roleId, int userId, int orderId, int cartItemId, CartItemForUpdateDto cartItem)
        {
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsAsync(userId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.OrderExistsForUser(userId, orderId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                return Forbid();
            }
            //if order has been placed and payment done, cart items can't be modified
            var order = await _vastraRepository.GetOrderAsync(orderId);
            if (order.PaymentStatus.Equals(PaymentStatus.Success.ToString()))
            {
                return BadRequest();
            }
            var cartItemEntity = await _vastraRepository.GetCartItemForOrderAsync(orderId, cartItemId);
            if (cartItemEntity == null)
            {
                return NotFound(cartItemId);
            }
            //check if modified quantity is available
            var product = await _vastraRepository.GetProductAsync(cartItemEntity.ProductId);
            if (product == null)
            {
                return NotFound();
            }
            if (cartItem.Quantity > product.Quantity)
            {
                return BadRequest();
            }
            
            float oldCartItemValue = cartItemEntity.Value;

            _mapper.Map(cartItem, cartItemEntity);

            //update Value of cart item
            cartItemEntity.Value = cartItem.Quantity * product.Price;

            //update Modified Time of cartItem
            cartItemEntity.DateModified = DateTime.Now;

            //update order value
            var oldOrderValue = order.Value;
            order.Value = oldOrderValue - oldCartItemValue + cartItemEntity.Value;


            await _vastraRepository.SaveChangesAsync();

            return NoContent();
        }
        [HttpPatch("{cartItemId}")]
        public async Task<ActionResult> PartiallyUpdateCartItem(int roleId, int userId, int orderId, int cartItemId,
            JsonPatchDocument<CartItemForUpdateDto> patchDocument)
        {
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsAsync(userId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.OrderExistsForUser(userId, orderId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                return Forbid();
            }
            //if order has been placed and payment done, cart items can't be modified
            var order = await _vastraRepository.GetOrderAsync(orderId);
            if (order.PaymentStatus.Equals(PaymentStatus.Success.ToString()))
            {
                return BadRequest();
            }
            var cartItemEntity = await _vastraRepository.GetCartItemForOrderAsync(orderId, cartItemId);
            if (cartItemEntity == null)
            {
                return NotFound(cartItemId);
            }
            //check if modified quantity is available
            var product = await _vastraRepository.GetProductAsync(cartItemEntity.ProductId);
            if (product == null)
            {
                return NotFound();
            }
            var cartItemToPatch = _mapper.Map<CartItemForUpdateDto>(cartItemEntity);
            if (cartItemToPatch.Quantity > product.Quantity)
            {
                return BadRequest();
            }

            float oldCartItemValue = cartItemEntity.Value;

            patchDocument.ApplyTo(cartItemToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!TryValidateModel(cartItemToPatch))
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(cartItemToPatch, cartItemEntity);

            //update value of cart item
            cartItemEntity.Value = cartItemToPatch.Quantity * product.Price;

            //update Modified Time of cartItem
            cartItemEntity.DateModified = DateTime.Now;

            //update order value
            var oldOrderValue = order.Value;
            order.Value = oldOrderValue - oldCartItemValue + cartItemEntity.Value;

            await _vastraRepository.SaveChangesAsync();

            


            return NoContent();
        }
        [HttpDelete("{cartItemId}")]
        public async Task<ActionResult> DeleteCartItem(int roleId, int userId, int orderId, int cartItemId)
        {
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsAsync(userId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.OrderExistsForUser(userId, orderId))
            {
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                return Forbid();
            }
            //if order has been placed and payment done, cart items can't be modified
            var order = await _vastraRepository.GetOrderAsync(orderId);
            if (order.PaymentStatus.Equals(PaymentStatus.Success.ToString()))
            {
                return BadRequest();
            }
            var cartItemToBeDeleted = await _vastraRepository.GetCartItemForOrderAsync(orderId, cartItemId);
            if (cartItemToBeDeleted == null)
            {
                return NotFound(cartItemId);
            }
            _vastraRepository.DeleteCartItem(cartItemToBeDeleted);

            //update order value
            var oldOrderValue = order.Value;
            order.Value = oldOrderValue - cartItemToBeDeleted.Value;

            await _vastraRepository.SaveChangesAsync();

            return NoContent();
        }

    }
}
