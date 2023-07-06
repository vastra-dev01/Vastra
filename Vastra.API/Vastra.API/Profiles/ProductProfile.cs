using AutoMapper;

namespace Vastra.API.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Entities.Product, Models.ProductDto>();
            CreateMap<Entities.Product, Models.ForCreationAndUpdate.ProductForCreationDto>();
            CreateMap<Models.ForCreationAndUpdate.ProductForCreationDto, Entities.Product>();
        }
    }
}
