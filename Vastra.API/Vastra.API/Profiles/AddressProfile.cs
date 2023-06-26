using AutoMapper;

namespace Vastra.API.Profiles
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<Entities.Address, Models.AddressDto>();
            CreateMap<Entities.Address, Models.ForCreationAndUpdate.AddressForCreationAndUpdateDto>();
            CreateMap<Models.ForCreationAndUpdate.AddressForCreationAndUpdateDto, Entities.Address>();
        }
    }
}
