using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ILogger<UsersController> _logger;
        const int maxUsersPageSize = 20;
        public UsersController(IVastraRepository vastraRepository,
            IMapper mapper,
            ILogger<UsersController> logger)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        [Authorize(Policy = "MustBeAdmin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(int roleId,
            string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogDebug($"Inside GetUsers in UsersController.");
            if(pageSize > maxUsersPageSize)
            {
                pageSize = maxUsersPageSize;
            }
            var (userEntities, paginationMetadata) = await _vastraRepository.GetUsersByRoleAsync(roleId, name, searchQuery, pageNumber, pageSize);
            _logger.LogInformation($"Total {userEntities.Count()} fetched " +
                $"in UsersController.");
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            return Ok(_mapper.Map<IEnumerable<UserDto>>(userEntities));
        }
        [HttpHead]
        [HttpGet("{userId}", Name = "GetUser")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUser(int roleId, int userId, bool includeAddresses = false, bool includeOrders = false)
        {
            _logger.LogDebug($"Inside GetUser in UsersController.");

            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug($"Role with id {roleId} was not found " +
                    $"in GetUser() " +
                    $"in UsersController.");
                return NotFound();
            }
            var userEntity = await _vastraRepository.GetUserByRoleAsync(roleId, userId, includeAddresses, includeOrders);
            if(userEntity == null)
            {
                _logger.LogDebug($"User with id {userId} was not found " +
                    $"in GetUser() " +
                    $"in UsersController.");
                return NotFound();
            }
            if(!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug($"User claim failed for userId {userId} " +
                   $"in GetUser() " +
                   $"in UsersController.");
                return Forbid();
            }
            if(includeAddresses && includeOrders)
            {
                return Ok(_mapper.Map<UserWithAddressesAndOrdersDto>(userEntity));
            }
            else if (includeAddresses)
            {
                _logger.LogInformation($"Successfully returning user with id {userId} " +
                    $"including addresses.");
                return Ok(_mapper.Map<UserWithAddressesDto>(userEntity));
            }
            else if (includeOrders)
            {
                _logger.LogInformation($"Successfully returning user with id {userId} " +
                    $"including orders.");
                return Ok(_mapper.Map<UserWithOrdersDto>(userEntity));
            }
            _logger.LogInformation($"Successfully returning user with id {userId}.");
            return Ok(_mapper.Map<UserDto>(userEntity));
        }
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(int roleId, UserForCreationDto user)
        {
            _logger.LogDebug($"Inside CreateUser in UsersController.");

            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug($"Role with id {roleId} was not found " +
                    $"in CreateUser() " +
                    $"in UsersController.");
                return NotFound();
            }
            var role = await _vastraRepository.GetRoleAsync(roleId);
            if(role.RoleName == "Admin") {
                if (!User.IsInRole("Admin"))
                {
                    _logger.LogDebug($"Can not create a user with role as 'Admin' " +
                        $"by a user with role as 'User'.");
                    return Forbid();
                }
            }
            var finalUser = _mapper.Map<Entities.User>(user);

            //set date added and date modified for new user
            finalUser.DateAdded = DateTime.Now;
            finalUser.DateModified = DateTime.Now;

            _logger.LogDebug($"Updated finalUser.DateAdded = {finalUser.DateAdded} " +
                $"& finalUser.DateModified = {finalUser.DateModified} " +
                $"in CreateUser() " +
                $"in UsersController.");

            //hash password
            var password = finalUser.Password;
            var hashedPassword = Hashing.GetSha256Hash(password);
            _logger.LogDebug($"Password hashing successfull " +
                $"in CreateUser() " +
                $"in UsersController.");
            finalUser.Password = hashedPassword;

            await _vastraRepository.AddUserForRole(roleId, finalUser);
            await _vastraRepository.SaveChangesAsync();

            var createdUserToReturn = _mapper.Map<Models.UserDto>(finalUser);

            _logger.LogInformation($"Successfully returning created user with id " +
                $"{createdUserToReturn.UserId}.");
            
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
        [Authorize]
        public async Task<ActionResult> UpdateUser(int roleId, int userId,
            UserForUpdateDto user)
        {
            _logger.LogDebug($"Inside UpdateUser in UsersController.");

            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug($"Role with id {roleId} was not found " +
                    $"in UpdateUser() " +
                    $"in UsersController.");
                return NotFound();
            }
            var userEntity = await _vastraRepository.GetUserByRoleAsync(roleId, userId);
            if (userEntity == null)
            {
                _logger.LogDebug($"User with id {userId} was not found " +
                    $"in UpdateUser() " +
                    $"in UsersController.");
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug($"User claim failed for userId {userId} " +
                   $"in UpdateUser() " +
                   $"in UsersController.");
                return Forbid();
            }
            _mapper.Map(user, userEntity);
            //update Modified Time of product
            userEntity.DateModified = DateTime.Now;

            _logger.LogDebug($"Updated userEntity.DateModified = {userEntity.DateModified} " +
                $"in UpdateUser() " +
                $"in UsersController.");

            await _vastraRepository.SaveChangesAsync();

            _logger.LogInformation($"Successfully updated user with id {userId}.");

            return NoContent();
        }
        [HttpPatch("{userId}")]
        [Authorize]
        public async Task<ActionResult> PartiallyUpdateUser(int roleId, int userId,
            JsonPatchDocument<UserForUpdateDto> patchDocument)
        {
            _logger.LogDebug($"Inside PartiallyUpdateUser in UsersController.");

            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug($"Role with id {roleId} was not found " +
                    $"in PartiallyUpdateUser() " +
                    $"in UsersController.");
                return NotFound();
            }
            var userEntity = await _vastraRepository.GetUserByRoleAsync(roleId, userId);
            if (userEntity == null)
            {
                _logger.LogDebug($"User with id {userId} was not found " +
                    $"in PartiallyUpdateUser() " +
                    $"in UsersController.");
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug($"User claim failed for userId {userId} " +
                   $"in PartiallyUpdateUser() " +
                   $"in UsersController.");
                return Forbid();
            }
            var userToPatch = _mapper.Map<UserForUpdateDto>(userEntity);
            patchDocument.ApplyTo(userToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                _logger.LogDebug($"Validation failed in " +
                    $"in PartiallyUpdateUser() " +
                    $"in UsersController.");
                return BadRequest(ModelState);
            }
            if (!TryValidateModel(userToPatch))
            {
                _logger.LogDebug($"Validation failed in " +
                    $"in PartiallyUpdateUser() " +
                    $"in UsersController.");
                return BadRequest(ModelState);
            }
            _mapper.Map(userToPatch, userEntity);
            //update Modified Time of product
            userEntity.DateModified = DateTime.Now;

            _logger.LogDebug($"Updated userEntity.DateModified = {userEntity.DateModified} " +
                $"in UpdateUser() " +
                $"in UsersController.");

            await _vastraRepository.SaveChangesAsync();

            _logger.LogInformation($"Successfully updated user with id {userId}.");

            return NoContent();

        }
        [HttpDelete("{userId}")]
        [Authorize]
        public async Task<ActionResult> DeleteUser(int roleId, int userId)
        {
            _logger.LogDebug($"Inside DeleteUser in UsersController.");

            if (!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug($"Role with id {roleId} was not found " +
                    $"in DeleteUser() " +
                    $"in UsersController.");
                return NotFound();
            }
            var userToDelete = await _vastraRepository.GetUserByRoleAsync(roleId, userId);
            if (userToDelete == null)
            {
                _logger.LogDebug($"User with id {userId} was not found " +
                    $"in DeleteUser() " +
                    $"in UsersController.");
                return NotFound();
            }
            if (!await _vastraRepository.ValidateUserClaim(User, userId))
            {
                _logger.LogDebug($"User claim failed for userId {userId} " +
                   $"in DeleteUser() " +
                   $"in UsersController.");
                return Forbid();
            }
            _vastraRepository.DeleteUser(userToDelete);
            await _vastraRepository.SaveChangesAsync();

            _logger.LogInformation($"Successfully deleted user with id {userId}.");


            return NoContent();
        }
    }
}
