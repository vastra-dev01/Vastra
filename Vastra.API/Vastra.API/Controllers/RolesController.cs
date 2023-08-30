using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vastra.API.Models;
using Vastra.API.Models.ForCreationAndUpdate;
using Vastra.API.Services;

namespace Vastra.API.Controllers
{
    [ApiController]
    [Route("api/roles")]
    [Authorize(Policy = "MustBeAdmin")]
    public class RolesController : ControllerBase
    {
        private readonly IVastraRepository _vastraRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RolesController> _logger;
        public RolesController(IVastraRepository vastraRepository,
            IMapper mapper,
            ILogger<RolesController> logger)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            _logger.LogDebug($"Inside GetRoles in RolesController.");
            var rolesList = await _vastraRepository.GetRolesAsync();
            _logger.LogInformation($"Total {rolesList.Count()} fetched in RolesController.");
            return Ok(_mapper.Map<IEnumerable<RoleDto>>(rolesList));
        }
        [HttpHead]
        [HttpGet("roleId", Name = "GetRole")]
        public async Task<ActionResult<RoleDto>> GetRole(int roleId)
        {
            _logger.LogDebug($"Inside GetRole in RolesController.");
            if(!await _vastraRepository.RoleExistsAsync(roleId))
            {
                _logger.LogDebug($"Role with id {roleId} was not found in " +
                    $"GetRole() " +
                    $"in RolesController.");
                return NotFound();
            }
            var role = await _vastraRepository.GetRoleAsync(roleId);

            _logger.LogInformation($"Successfully fetched role with id {roleId} in RolesController."); ;
            return _mapper.Map<RoleDto>(role);
        }
        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole(RoleForCreationDto role)
        {
            _logger.LogDebug($"Inside CreateRole in RolesController.");
            var finalRole = _mapper.Map<Entities.Role>(role);
            //set date addedd and date modified for newly created role
            finalRole.DateAdded = DateTime.Now;
            finalRole.DateModified = DateTime.Now;

            _logger.LogDebug($"Updated finalRole.DateAdded = {finalRole.DateAdded} " +
                $"& finalRole.DateModified = {finalRole.DateModified} " +
                $"in CreateRole() " +
                $"in RolesController.");

            await _vastraRepository.AddRoleAsync(finalRole);
            await _vastraRepository.SaveChangesAsync();

            var createdRoleToReturn = _mapper.Map<RoleDto>(finalRole);

            _logger.LogInformation($"Successfully returning created role with id " +
                $"{createdRoleToReturn.RoleId}.");

            return CreatedAtRoute("GetRole",
                new
                {
                    roleId = createdRoleToReturn.RoleId
                },
                createdRoleToReturn
                ); ;
        }
    }
}
