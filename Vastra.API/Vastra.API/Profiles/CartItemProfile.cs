using AutoMapper;

namespace Vastra.API.Profiles
{
    public class CartItemProfile : Profile
    {
        public CartItemProfile()
        {
            CreateMap<Entities.CartItem, Models.CartItemDto>();
            CreateMap<Entities.CartItem, Models.ForCreationAndUpdate.CartItemForCreationAndUpdateDto>();
            CreateMap<Models.ForCreationAndUpdate.CartItemForCreationAndUpdateDto, Entities.CartItem>();
        }
    }
}
