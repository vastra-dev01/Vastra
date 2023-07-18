using AutoMapper;

namespace Vastra.API.Profiles
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<Entities.Address, Models.AddressDto>();
            CreateMap<Entities.Address, Models.ForCreationAndUpdate.AddressForCreationDto>();
            CreateMap<Entities.Address, Models.ForCreationAndUpdate.AddressForUpdateDto>();
            CreateMap<Models.ForCreationAndUpdate.AddressForCreationDto, Entities.Address>();
            CreateMap<Models.ForCreationAndUpdate.AddressForUpdateDto, Entities.Address>();
        }
    }
}
