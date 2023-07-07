using AutoMapper;

namespace Vastra.API.Profiles
{
    public class CartItemProfile : Profile
    {
        public CartItemProfile()
        {
            CreateMap<Entities.CartItem, Models.CartItemDto>();
            CreateMap<Entities.CartItem, Models.ForCreationAndUpdate.CartItemForCreationDto>();
            CreateMap<Entities.CartItem, Models.ForCreationAndUpdate.CartItemForUpdateDto>();
            CreateMap<Models.ForCreationAndUpdate.CartItemForCreationDto, Entities.CartItem>();
            CreateMap<Models.ForCreationAndUpdate.CartItemForUpdateDto, Entities.CartItem>();
        }
    }
}
