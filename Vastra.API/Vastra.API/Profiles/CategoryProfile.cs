using AutoMapper;

namespace Vastra.API.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Entities.Category, Models.CategoryDto>();
            CreateMap<Entities.Category, Models.CategoryWithoutChildCategoriesDto>();
            CreateMap<Entities.Category, Models.CategoryWithoutProductsDto>();
            CreateMap<Entities.Category, Models.ForCreationAndUpdate.CategoryForCreationAndUpdateDto>();
            CreateMap<Models.ForCreationAndUpdate.CategoryForCreationAndUpdateDto, Entities.Category>();
        }
    }
}
