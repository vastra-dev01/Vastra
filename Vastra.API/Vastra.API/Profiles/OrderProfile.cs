using AutoMapper;

namespace Vastra.API.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Entities.Order, Models.OrderDto>();
            CreateMap<Entities.Order, Models.OrderWithoutCartItemsDto>();
            CreateMap<Entities.Order, Models.UserWithoutOrdersDto>();
            CreateMap<Entities.Order, Models.ForCreationAndUpdate.OrderForCreationAndUpdateDto>();
            CreateMap<Models.ForCreationAndUpdate.OrderForCreationAndUpdateDto, Entities.Order>();
        }
    }
}
