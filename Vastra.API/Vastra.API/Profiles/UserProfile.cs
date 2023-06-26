using AutoMapper;

namespace Vastra.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Entities.User, Models.UserDto>();
            CreateMap<Entities.User, Models.UserWithoutAddressesDto>();
            CreateMap<Entities.User, Models.UserWithoutOrdersDto>();
            CreateMap<Models.ForCreationAndUpdate.UserForCreationAndUpdateDto, Entities.User>();
            CreateMap<Entities.User, Models.ForCreationAndUpdate.UserForCreationAndUpdateDto>();

        }
    }
}
