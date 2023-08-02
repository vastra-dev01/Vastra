using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Vastra.API.Models;
using Vastra.API.Models.ForCreationAndUpdate;
using Vastra.API.Services;

namespace Vastra.API.Controllers
{
    [ApiController]
    [Route("api/roles/{roleId}/users")]
    public class UsersController : ControllerBase
    {
        private readonly IVastraRepository _vastraRepository;
        private readonly IMapper _mapper;
        const int maxUsersPageSize = 20;
        public UsersController(IVastraRepository vastraRepository,
            IMapper mapper)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(int roleId,
            string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if(pageSize > maxUsersPageSize)
            {
                pageSize = maxUsersPageSize;
            }
            var (userEntities, paginationMetadata) = await _vastraRepository.GetUsersByRoleAsync(roleId, name, searchQuery, pageNumber, pageSize);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            return Ok(_mapper.Map<IEnumerable<UserDto>>(userEntities));
        }
        [HttpHead]
        [HttpGet("{userId}", Name = "GetUser")]
        public async Task<ActionResult<UserDto>> GetUser(int roleId, int userId, bool includeAddresses = false, bool includeOrders = false)
        {
            if(!await _vastraRepository.RoleExistsAsync(roleId))
            {
                return NotFound();
            }
            var userEntity = await _vastraRepository.GetUserByRoleAsync(roleId, userId, includeAddresses, includeOrders);
            if(includeAddresses && includeOrders)
            {
                return Ok(_mapper.Map<UserWithAddressesAndOrdersDto>(userEntity));
            }
            else if (includeAddresses)
            {
                return Ok(_mapper.Map<UserWithAddressesDto>(userEntity));
            }
            else if (includeOrders)
            {
                return Ok(_mapper.Map<UserWithOrdersDto>(userEntity));
            }
            return Ok(_mapper.Map<UserDto>(userEntity));
        }
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(int roleId, UserForCreationDto user)
        {
            if(!await _vastraRepository.RoleExistsAsync(roleId))
            {
                return NotFound();
            }
            var finalUser = _mapper.Map<Entities.User>(user);

            //set date added and date modified for new user
            finalUser.DateAdded = DateTime.Now;
            finalUser.DateModified = DateTime.Now;
            //hash password
            var password = finalUser.Password;
            var hashedPassword = Hashing.GetSha256Hash(password);
            finalUser.Password = hashedPassword;

            await _vastraRepository.AddUserForRole(roleId, finalUser);
            await _vastraRepository.SaveChangesAsync();

            var createdUserToReturn = _mapper.Map<Models.UserDto>(finalUser);
            return CreatedAtRoute("GetUser",
                new
                {
                    roleId = roleId,
                    userId = createdUserToReturn.UserId
                },
                createdUserToReturn
                );
        }
        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdateUser(int roleId, int userId,
            UserForUpdateDto user)
        {
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                return NotFound();
            }
            var userEntity = await _vastraRepository.GetUserByRoleAsync(roleId, userId);
            if (userEntity == null)
            {
                return NotFound();
            }
            _mapper.Map(user, userEntity);
            //update Modified Time of product
            userEntity.DateModified = DateTime.Now;
            await _vastraRepository.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{userId}")]
        public async Task<ActionResult> PartiallyUpdateUser(int roleId, int userId,
            JsonPatchDocument<UserForUpdateDto> patchDocument)
        {
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                return NotFound();
            }
            var userEntity = await _vastraRepository.GetUserByRoleAsync(roleId, userId);
            if (userEntity == null)
            {
                return NotFound();
            }
            var userToPatch = _mapper.Map<UserForUpdateDto>(userEntity);
            patchDocument.ApplyTo(userToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!TryValidateModel(userToPatch))
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(userToPatch, userEntity);
            //update Modified Time of product
            userEntity.DateModified = DateTime.Now;
            await _vastraRepository.SaveChangesAsync();
            return NoContent();

        }
        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUser(int roleId, int userId)
        {
            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                return NotFound();
            }
            var userToDelete = await _vastraRepository.GetUserByRoleAsync(roleId, userId);
            if (userToDelete == null)
            {
                return NotFound();
            }
            _vastraRepository.DeleteUser(userToDelete);
            await _vastraRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
