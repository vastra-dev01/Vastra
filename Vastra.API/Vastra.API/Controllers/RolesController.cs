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
        public RolesController(IVastraRepository vastraRepository,
            IMapper mapper)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var rolesList = await _vastraRepository.GetRolesAsync();
            return Ok(_mapper.Map<IEnumerable<RoleDto>>(rolesList));
        }
        [HttpHead]
        [HttpGet("roleId", Name = "GetRole")]
        public async Task<ActionResult<RoleDto>> GetRole(int roleId)
        {
            if(!await _vastraRepository.RoleExistsAsync(roleId))
            {
                return NotFound();
            }
            var role = await _vastraRepository.GetRoleAsync(roleId);
            return _mapper.Map<RoleDto>(role);
        }
        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole(RoleForCreationDto role)
        {
            var finalRole = _mapper.Map<Entities.Role>(role);
            //set date addedd and date modified for newly created role
            finalRole.DateAdded = DateTime.Now;
            finalRole.DateModified = DateTime.Now;

            await _vastraRepository.AddRoleAsync(finalRole);
            await _vastraRepository.SaveChangesAsync();

            var createdRoleToReturn = _mapper.Map<RoleDto>(finalRole);

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
