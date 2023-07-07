using AutoMapper;

namespace Vastra.API.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Entities.Product, Models.ProductDto>();
            CreateMap<Entities.Product, Models.ForCreationAndUpdate.ProductForCreationDto>();
            CreateMap<Entities.Product, Models.ForCreationAndUpdate.ProductForUpdateDto>();
            CreateMap<Models.ForCreationAndUpdate.ProductForCreationDto, Entities.Product>();
            CreateMap<Models.ForCreationAndUpdate.ProductForUpdateDto, Entities.Product>();
        }
    }
}
