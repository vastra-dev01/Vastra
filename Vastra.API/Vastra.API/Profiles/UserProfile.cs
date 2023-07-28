using AutoMapper;

namespace Vastra.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Entities.User, Models.UserDto>();
            CreateMap<Entities.User, Models.UserWithOrdersDto>();
            CreateMap<Entities.User, Models.UserWithAddressesDto>();
            CreateMap<Entities.User, Models.UserWithAddressesAndOrdersDto>();
            CreateMap<Models.ForCreationAndUpdate.UserForCreationDto, Entities.User>();
            CreateMap<Models.ForCreationAndUpdate.UserForUpdateDto, Entities.User>();
            CreateMap<Entities.User, Models.ForCreationAndUpdate.UserForCreationDto>();
            CreateMap<Entities.User, Models.ForCreationAndUpdate.UserForUpdateDto>();

        }
    }
}
