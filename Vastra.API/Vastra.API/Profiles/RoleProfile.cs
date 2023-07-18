using AutoMapper;

namespace Vastra.API.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Entities.Role, Models.RoleDto>();
            CreateMap<Entities.Role, Models.ForCreationAndUpdate.RoleForCreationDto>();
            CreateMap<Entities.Role, Models.ForCreationAndUpdate.RoleForUpdateDto>();
            CreateMap<Models.ForCreationAndUpdate.RoleForCreationDto, Entities.Role>();
            CreateMap<Models.ForCreationAndUpdate.RoleForUpdateDto, Entities.Role>();
        }
    }
}
