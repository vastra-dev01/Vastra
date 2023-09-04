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
    [ApiController]
    [Route("api/roles/{roleId}/users/{userId}/addresses")]
    public class AddressesController : Controller
    {
        private readonly IVastraRepository _vastraRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddressesController> _logger;
        const int maxAddressesPageSize = 10;
        public AddressesController(IVastraRepository vastraRepository, IMapper mapper, ILogger<AddressesController> logger)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AddressDto>>> GetAddresses(int roleId, int userId, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogDebug("Inside GetAddresses in AddressesController.");

            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug("Role with role id {0} was not found in AddressesController.", roleId);
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsWithRoleAsync(roleId, userId))
            {
                _logger.LogDebug("UserId {0} with role id {1} was not found in AddressesController.", userId, roleId);
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug("User claim failed for userId {0} in AddressesController.", userId);
                return Forbid();
            }
            if (pageSize > maxAddressesPageSize)
            {
                pageSize = maxAddressesPageSize;
            }
            var (addressEntities, paginationMetadata) = await _vastraRepository.GetAddressesForUserAsync(userId, pageNumber, pageSize);
            
            //Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            _logger.LogInformation("Total {0} addresses fetched for  user {1} in AddressesController", addressEntities.Count(), userId);
            
            return Ok(_mapper.Map<IEnumerable<AddressDto>>(addressEntities));

        }
        [HttpHead("{addressId}")]
        [HttpGet("{addressId}", Name = "GetAddress")]
        [Authorize]
        public async Task<IActionResult> GetAddress(int roleId, int userId, int addressId)
        {
            _logger.LogDebug("Inside GetAddress in AddressesController.");

            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug("Role with role id {0} was not found in AddressesController.", roleId);
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsWithRoleAsync(roleId, userId))
            {
                _logger.LogDebug("UserId {0} with role id {1} was not found in AddressesController.", userId, roleId);
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug("User claim failed for userId {0} in AddressesController.", userId);
                return Forbid();
            }
            var address = await _vastraRepository.GetAddressForUserAsync(userId, addressId);
            if (address == null)
            {
                _logger.LogDebug("Address with addressId {0} and userId {1} was not found in AddressesController", addressId, userId);
                return NotFound();
            }
            _logger.LogInformation("Address with addressId {0} fetched for userId {1}", addressId, userId);
            return Ok(_mapper.Map<AddressDto>(address));
            
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AddressDto>> CreateAddress(int roleId, int userId, AddressForCreationDto address)
        {
            _logger.LogDebug("Inside CreateAddress in AddressesController.");

            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug("Role with role id {0} was not found in AddressesController.", roleId);
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsWithRoleAsync(roleId, userId))
            {
                _logger.LogDebug("UserId {0} with role id {1} was not found in AddressesController.", userId, roleId);
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug("User claim failed for userId {0} in AddressesController.", userId);
                return Forbid();
            }
            var finaladdress = _mapper.Map<Entities.Address>(address);
            //set date added and date modified for  newly created address
            finaladdress.DateAdded = DateTime.Now;
            finaladdress.DateModified = DateTime.Now;

            _logger.LogDebug("Date Added = {DT} and Date Modified = {DT} set for newly created address in AddressesController.",
                finaladdress.DateAdded, finaladdress.DateModified);


            var (addresses, paginationMetadata) = await _vastraRepository.GetAddressesForUserAsync(userId, 1, 100);
            if (addresses.Count() == 0)
            {
                _logger.LogDebug("addresses.Count() = 0 in CreateAddress() in AddressesController");
                finaladdress.Type = (int)AddressType.Primary;
            }
            await _vastraRepository.AddAddressForUserAsync(userId, finaladdress);
            
            //if address set as primary, set other addresses as secondary
            if (finaladdress.Type == (int)AddressType.Primary && addresses.Count() > 0)
            {
                _logger.LogDebug("Found {0} addresses for user {1} to be set as secondary in AddressesController.", addresses.Count(), userId);
                foreach (var addressMember in addresses)
                {
                    addressMember.Type = (int)AddressType.Secondary;
                }
            }
            await _vastraRepository.SaveChangesAsync();

            var createdAddressToReturn = _mapper.Map<AddressDto>(finaladdress);

            _logger.LogInformation("Created new address : {0}", JsonSerializer.Serialize(createdAddressToReturn));
            return CreatedAtRoute("GetAddress",
                new
                {
                    roleId = roleId,
                    userId = userId,
                    addressId = createdAddressToReturn.AddressId
                },
                createdAddressToReturn

                );
        }
        [Authorize]
        [HttpPatch("{addressId}")]
        public async Task<ActionResult> PartiallyUpdateAddress(int roleId, int userId, int addressId,
            JsonPatchDocument<AddressForUpdateDto> patchDocument)
        {
            _logger.LogDebug("Inside PartiallyUpdateAddress in AddressesController.");

            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogInformation("Role with role id {0} was not found in AddressesController.", roleId);
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsWithRoleAsync(roleId, userId))
            {
                _logger.LogInformation("UserId {0} with role id {1} was not found in AddressesController.", userId, roleId);
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogInformation("User claim failed for userId {0} in AddressesController.", userId);
                return Forbid();
            }
            var addressEntity = await _vastraRepository.GetAddressForUserAsync(userId, addressId);
            if(addressEntity == null)
            {
                _logger.LogInformation("Address with addressId {0} for userId {1} was not found in AddressesController", 
                    addressId, userId);
                return NotFound();
            }
            var addressToPatch = _mapper.Map<AddressForUpdateDto>(addressEntity);
            patchDocument.ApplyTo(addressToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Model Validation failed for address in AddressesController: {0}", 
                    JsonSerializer.Serialize(ModelState));
                return BadRequest(ModelState);
            }
            if (!TryValidateModel(addressToPatch))
            {
                _logger.LogInformation("Model Validation failed for address in AddressesController: {0}",
                    JsonSerializer.Serialize(ModelState));
                return BadRequest(ModelState);
            }
            _mapper.Map(addressToPatch, addressEntity);
            //update Modified Time of address
            addressEntity.DateModified = DateTime.Now;
            _logger.LogInformation("Date Modified = {DT} set for addressId {1} in AddressesController.",
                addressEntity.DateModified, addressId);
            //if address set as primary, set other addresses as secondary
            if (addressEntity.Type == (int)AddressType.Primary)
            {
                var (addresses, paginationMetadata) = await _vastraRepository.GetAddressesForUserAsync(userId,1,100);
                _logger.LogDebug("Found {0} addresses for userId {1} in PartiallyUpdateAddress in AddressesController"
                    , addresses.Count(), userId);
                foreach (var address in addresses)
                {
                    if(addressEntity.AddressId != address.AddressId)
                    {
                        address.Type = (int)AddressType.Secondary;
                    }
                }
            }


            await _vastraRepository.SaveChangesAsync();
            _logger.LogInformation("Address with addressId {0} partially updated.", addressId);
            return NoContent();
        }
        [Authorize]
        [HttpPut("{addressId}")]
        public async Task<ActionResult> UpdateAddress(int roleId, int userId, int addressId, AddressForUpdateDto address)
        {
            _logger.LogDebug("Inside UpdateAddress in AddressesController.");

            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug("Role with role id {0} was not found in AddressesController.", roleId);
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsWithRoleAsync(roleId, userId))
            {
                _logger.LogDebug("UserId {0} with role id {1} was not found in AddressesController.", userId, roleId);
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug("User claim failed for userId {0} in AddressesController.", userId);
                return Forbid();
            }
            var addressEntity = await _vastraRepository.GetAddressForUserAsync(userId, addressId);
            if (addressEntity == null)
            {
                _logger.LogDebug("Address with addressId {0} was not found in AddressesController", addressId);
                return NotFound();
            }
            _mapper.Map(address, addressEntity);
            //update Modified Time of product
            addressEntity.DateModified = DateTime.Now;
            _logger.LogDebug("Date Modified = {DT} set for addressId {1} in AddressesController.",
                addressEntity.DateModified, addressId);
            //if address set as primary, set other addresses as secondary
            if (addressEntity.Type == (int)AddressType.Primary)
            {
                var (addresses, paginationMetadata) = await _vastraRepository.GetAddressesForUserAsync(userId, 1, 100);
                _logger.LogDebug("Found {0} addresses for userId {1} in UpdateAddress in AddressesController"
                     , addresses.Count(), userId);
                foreach (var addressMember in addresses)
                {
                    if (addressEntity.AddressId != addressMember.AddressId)
                    {
                        addressMember.Type = (int)AddressType.Secondary;
                    }
                }
            }
            await _vastraRepository.SaveChangesAsync();
            _logger.LogInformation("Address with addressId {0} updated.", addressId);
            return NoContent();
        }
        [HttpDelete("{addressId}")]
        [Authorize]
        public async Task<ActionResult> DeleteAddress(int roleId, int userId, int addressId)
        {
            _logger.LogDebug("Inside DeleteAddress in AddressesController.");

            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug("Role with role id {0} was not found in AddressesController.", roleId);
                return NotFound();
            }
            if (!await _vastraRepository.UserExistsWithRoleAsync(roleId, userId))
            {
                _logger.LogDebug("UserId {0} with role id {1} was not found in AddressesController.", userId, roleId);
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug("User claim failed for userId {0} in AddressesController.", userId);
                return Forbid();
            }
            var addressToDelete = await _vastraRepository.GetAddressForUserAsync(userId, addressId);
            if (addressToDelete == null)
            {
                _logger.LogDebug("Address with addressId {0} was not found in AddressesController", addressId);
                return NotFound();
            }
            var (addresses, paginationMetadata) = await _vastraRepository.GetAddressesForUserAsync(userId, 1, 100);
            _logger.LogDebug("Found {0} addresses for userId {1} in DeleteAddress in AddressesController"
                , addresses.Count(), userId);


            Address addressToBeSet = null;
            if (addressToDelete.Type == (int)AddressType.Primary && addresses.Count() > 1)
            {
                foreach(Address address in addresses)
                {
                    if(address.AddressId != addressToDelete.AddressId)
                    {
                        addressToBeSet = address;
                        break;
                    }
                }
            }
            if(addressToBeSet != null)
            {
                _logger.LogDebug("Setting address with addressId {0} as Primary in DeleteAddress in AddressesController"
                    , addressToBeSet.AddressId);
                addressToBeSet.Type = (int)AddressType.Primary;
            }
            _vastraRepository.DeleteAddress(addressToDelete);
            await _vastraRepository.SaveChangesAsync();
            _logger.LogInformation("Address with addressId {0} deleted.", addressId);
            return NoContent();
        }

    }
}
