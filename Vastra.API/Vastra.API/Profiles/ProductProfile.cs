using AutoMapper;

namespace Vastra.API.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Entities.Product, Models.ProductDto>();
            CreateMap<Entities.Product, Models.ForCreationAndUpdate.ProductForCreationAndUpdateDto>();
            CreateMap<Models.ForCreationAndUpdate.ProductForCreationAndUpdateDto, Entities.Product>();
        }
    }
}
