using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Vastra.API.Entities;
using Vastra.API.Models;
using Vastra.API.Models.ForCreationAndUpdate;
using Vastra.API.Services;

namespace Vastra.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IVastraRepository _vastraRepository;
        private readonly IMapper _mapper;
        const int maxCategoriesPageSize = 10;
        public CategoriesController(IVastraRepository vastraRepository, IMapper mapper)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > maxCategoriesPageSize)
            {
                pageSize = maxCategoriesPageSize;
            }
            var (categoryEntities, paginationMetadata) = await _vastraRepository.GetCategoriesAsync(pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categoryEntities));
        }
        [HttpGet("{categoryId}", Name = "GetCategory")]
        public async Task<IActionResult> GetCategory(int categoryId, bool includeChildCategories = false, bool includeProducts = false)
        {
            if(!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                return NotFound();
            }
            var categoryEntity = await _vastraRepository.GetCategoryAsync(categoryId, includeChildCategories, includeProducts);
            if(includeProducts && includeChildCategories)
            {
                return Ok(_mapper.Map<CategoryWithProductsAndCategoriesDto>(categoryEntity));
            }
            else if (includeProducts)
            {
                return Ok(_mapper.Map<CategoryWithProductsDto>(categoryEntity));
            }
            else if (includeChildCategories)
            {
                return Ok(_mapper.Map<CategoryWithChildCategoriesDto>(categoryEntity));
            }
            return Ok(_mapper.Map<CategoryDto>(categoryEntity));
        }
        [HttpPost]
        [Authorize(Policy = "MustBeAdmin")]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CategoryForCreationDto category)
        {
            var finalCategory = _mapper.Map<Entities.Category>(category);

            //add date added and date modified for new category
            finalCategory.DateAdded = DateTime.Now;
            finalCategory.DateModified = DateTime.Now;
            await _vastraRepository.AddCategoryAsync(finalCategory);
            await _vastraRepository.SaveChangesAsync();
            var createdCategoryToReturn = _mapper.Map<Models.CategoryDto>(finalCategory);
            return CreatedAtRoute("GetCategory",
                new
                {
                    categoryId = createdCategoryToReturn.CategoryId
                },
                createdCategoryToReturn
                );
        }
        [HttpPut("{categoryId}")]
        [Authorize(Policy = "MustBeAdmin")]
        public async Task<ActionResult> UpdateCategory(int categoryId,
            CategoryForUpdateDto category)
        {
            if (!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                return NotFound();
            }
            var categoryEntity = await _vastraRepository.GetCategoryAsync(categoryId);
            if (categoryEntity == null)
            {
                return NotFound();
            }
            _mapper.Map(category, categoryEntity);
            //update Modified Time of product
            categoryEntity.DateModified = DateTime.Now;
            await _vastraRepository.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{categoryId}")]
        [Authorize(Policy = "MustBeAdmin")]
        public async Task<ActionResult> PartiallyUpdateCategory(int categoryId,
            JsonPatchDocument<CategoryForUpdateDto> patchDocument)
        {
            if (!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                return NotFound();
            }
            var categoryEntity = await _vastraRepository.GetCategoryAsync(categoryId);
            if (categoryEntity == null)
            {
                return NotFound();
            }
            var categoryToPatch = _mapper.Map<CategoryForUpdateDto>(categoryEntity);
            patchDocument.ApplyTo(categoryToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!TryValidateModel(categoryToPatch))
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(categoryToPatch, categoryEntity);
            //update Modified Time of category
            categoryEntity.DateModified = DateTime.Now;
            await _vastraRepository.SaveChangesAsync();
            return NoContent();

        }
        [HttpDelete("{categoryId}")]
        [Authorize(Policy = "MustBeAdmin")]
        public async Task<ActionResult> DeleteCategory(int categoryId) {
            if(!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                return NotFound();
            }
            var categoryToDelete = await _vastraRepository.GetCategoryAsync(categoryId);
            _vastraRepository.DeleteCategory(categoryToDelete);
            await _vastraRepository.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("{categoryId}/subCategories")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetChildCategoriesForCategory(int categoryId)
        {
            if(!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                return NotFound();
            }
            var childCategories = await _vastraRepository.GetChildCategoriesForCategoryAsync(categoryId);
            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(childCategories));
        }
        //[HttpGet("{categoryId}/subCategories/{subCategoryId}")]
        //public async Task<ActionResult<CategoryDto>>  -- This is not needed currently as indivisual category can be fetched by GetCategory GET method

        [HttpPost("{categoryId}/subCategories")]
        [Authorize(Policy = "MustBeAdmin")]
        public async Task<ActionResult<CategoryDto>> CreateSubCategory(int categoryId, CategoryForCreationDto subCategory)
        {
            if(!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                return NotFound();
            }
            var finalSubcategory = _mapper.Map<Entities.Category>(subCategory);
            //set date added  & date modified for new sub category
            finalSubcategory.DateAdded = DateTime.Now;
            finalSubcategory.DateModified = DateTime.Now;

            await _vastraRepository.AddChildCategoryForCategoryAsync(categoryId, finalSubcategory);
            await _vastraRepository.SaveChangesAsync();
            var createdSubCategoryToReturn = _mapper.Map<CategoryDto>(finalSubcategory);
            return CreatedAtRoute("GetCategory",
                new
                {
                    categoryId = createdSubCategoryToReturn.CategoryId
                },
                createdSubCategoryToReturn
                );
        }
    }
}
