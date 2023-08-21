using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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
        const int maxAddressesPageSize = 10;
        public AddressesController(IVastraRepository vastraRepository, IMapper mapper)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AddressDto>>> GetAddresses(int roleId, int userId, int pageNumber = 1, int pageSize = 10)
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
            if (pageSize > maxAddressesPageSize)
            {
                pageSize = maxAddressesPageSize;
            }
            var (addressEntities, paginationMetadata) = await _vastraRepository.GetAddressesForUserAsync(userId, pageNumber, pageSize);

            return Ok(_mapper.Map<IEnumerable<AddressDto>>(addressEntities));
        }
        [HttpHead("{addressId}")]
        [HttpGet("{addressId}", Name = "GetAddress")]
        [Authorize]
        public async Task<IActionResult> GetAddress(int roleId, int userId, int addressId)
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
            var address = await _vastraRepository.GetAddressForUserAsync(userId, addressId);
            if (address == null)
            {
                return NotFound();
            }
            
            return Ok(_mapper.Map<AddressDto>(address));
            
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AddressDto>> CreateAddress(int roleId, int userId, AddressForCreationDto address)
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
            var finaladdress = _mapper.Map<Entities.Address>(address);

            finaladdress.DateAdded = DateTime.Now;
            finaladdress.DateModified = DateTime.Now;

            var (addresses, paginationMetadata) = await _vastraRepository.GetAddressesForUserAsync(userId, 1, 100);
            if (addresses.Count() == 0)
            {
                finaladdress.Type = (int)AddressType.Primary;
            }
            await _vastraRepository.AddAddressForUserAsync(userId, finaladdress);
            
            //if address set as primary, set other addresses as secondary
            if (finaladdress.Type == (int)AddressType.Primary && addresses.Count() > 0)
            {
                foreach (var addressMember in addresses)
                {
                    addressMember.Type = (int)AddressType.Secondary;
                }
            }
            await _vastraRepository.SaveChangesAsync();

            var createdAddressToReturn = _mapper.Map<AddressDto>(finaladdress);
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
            var addressEntity = await _vastraRepository.GetAddressForUserAsync(userId, addressId);
            if(addressEntity == null)
            {
                return NotFound();
            }
            var addressToPatch = _mapper.Map<AddressForUpdateDto>(addressEntity);
            patchDocument.ApplyTo(addressToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!TryValidateModel(addressToPatch))
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(addressToPatch, addressEntity);
            //update Modified Time of address
            addressEntity.DateModified = DateTime.Now;
            
            //if address set as primary, set other addresses as secondary
            if(addressEntity.Type == (int)AddressType.Primary)
            {
                var (addresses, paginationMetadata) = await _vastraRepository.GetAddressesForUserAsync(userId,1,100);
                foreach (var address in addresses)
                {
                    if(addressEntity.AddressId != address.AddressId)
                    {
                        address.Type = (int)AddressType.Secondary;
                    }
                }
            }
            await _vastraRepository.SaveChangesAsync();
            return NoContent();
        }
        [Authorize]
        [HttpPut("{addressId}")]
        public async Task<ActionResult> UpdateAddress(int roleId, int userId, int addressId, AddressForUpdateDto address)
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
            var addressEntity = await _vastraRepository.GetAddressForUserAsync(userId, addressId);
            if (addressEntity == null)
            {
                return NotFound();
            }
            _mapper.Map(address, addressEntity);
            //update Modified Time of product
            addressEntity.DateModified = DateTime.Now;
            //if address set as primary, set other addresses as secondary
            if (addressEntity.Type == (int)AddressType.Primary)
            {
                var (addresses, paginationMetadata) = await _vastraRepository.GetAddressesForUserAsync(userId, 1, 100);
                foreach (var addressMember in addresses)
                {
                    if (addressEntity.AddressId != addressMember.AddressId)
                    {
                        addressMember.Type = (int)AddressType.Secondary;
                    }
                }
            }
            await _vastraRepository.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{addressId}")]
        [Authorize]
        public async Task<ActionResult> DeleteAddress(int roleId, int userId, int addressId)
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
            var addressToDelete = await _vastraRepository.GetAddressForUserAsync(userId, addressId);
            if (addressToDelete == null)
            {
                return NotFound();
            }
            var (addresses, paginationMetadata) = await _vastraRepository.GetAddressesForUserAsync(userId, 1, 100);
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
                addressToBeSet.Type = (int)AddressType.Primary;
            }
            _vastraRepository.DeleteAddress(addressToDelete);
            await _vastraRepository.SaveChangesAsync();
            return NoContent();
        }

    }
}
