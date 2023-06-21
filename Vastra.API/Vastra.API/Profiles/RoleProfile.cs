using AutoMapper;

namespace Vastra.API.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Entities.Role, Models.RoleDto>();
            CreateMap<Entities.Role, Models.ForCreationAndUpdate.RoleForCreationAndUpdateDto>();
            CreateMap<Models.ForCreationAndUpdate.RoleForCreationAndUpdateDto, Entities.Role>();
        }
    }
}
