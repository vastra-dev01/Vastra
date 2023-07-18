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
            CreateMap<Models.ForCreationAndUpdate.UserForCreationDto, Entities.User>();
            CreateMap<Models.ForCreationAndUpdate.UserForUpdateDto, Entities.User>();
            CreateMap<Entities.User, Models.ForCreationAndUpdate.UserForCreationDto>();
            CreateMap<Entities.User, Models.ForCreationAndUpdate.UserForUpdateDto>();

        }
    }
}
