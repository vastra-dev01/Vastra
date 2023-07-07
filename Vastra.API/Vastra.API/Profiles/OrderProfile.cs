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
            CreateMap<Entities.Order, Models.ForCreationAndUpdate.OrderForCreationDto>();
            CreateMap<Entities.Order, Models.ForCreationAndUpdate.OrderForUpdateDto>();
            CreateMap<Models.ForCreationAndUpdate.OrderForCreationDto, Entities.Order>();
            CreateMap<Models.ForCreationAndUpdate.OrderForUpdateDto, Entities.Order>();
        }
    }
}
