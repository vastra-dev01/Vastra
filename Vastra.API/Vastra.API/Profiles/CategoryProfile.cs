using AutoMapper;

namespace Vastra.API.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Entities.Category, Models.CategoryDto>();
            CreateMap<Entities.Category, Models.CategoryWithChildCategoriesDto>();
            CreateMap<Entities.Category, Models.CategoryWithProductsDto>();
            CreateMap<Entities.Category, Models.ForCreationAndUpdate.CategoryForUpdateDto>();
            CreateMap<Entities.Category, Models.ForCreationAndUpdate.CategoryForCreationDto>();
            CreateMap<Models.ForCreationAndUpdate.CategoryForUpdateDto, Entities.Category>();
            CreateMap<Models.ForCreationAndUpdate.CategoryForCreationDto, Entities.Category>();
        }
    }
}
