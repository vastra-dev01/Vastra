using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Vastra.API.Entities;
using Vastra.API.Models;
using Vastra.API.Models.CustomException;
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
        private readonly ILogger<CategoriesController> _logger;
        const int maxCategoriesPageSize = 10;

        public CategoriesController(IVastraRepository vastraRepository, IMapper mapper, ILogger<CategoriesController> logger)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogDebug($"Inside GetCategories in CategoriesController.");
            if (pageSize > maxCategoriesPageSize)
            {
                pageSize = maxCategoriesPageSize;
            }
            var (categoryEntities, paginationMetadata) = await _vastraRepository.GetCategoriesAsync(pageNumber, pageSize);

            _logger.LogInformation($"Total {categoryEntities.Count()} categories fetched in CategoriesController");
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categoryEntities));
        }
        [HttpGet("{categoryId}", Name = "GetCategory")]
        public async Task<IActionResult> GetCategory(int categoryId, bool includeChildCategories = false, 
            bool includeProducts = false)
        {
            _logger.LogDebug($"Inside GetCategory in CategoriesController.");
            if (!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                _logger.LogDebug($"Category with id {categoryId} not found" +
                    $"in GetCategory() in CategoriesController");
                return NotFound();
            }
            var categoryEntity = await _vastraRepository.GetCategoryAsync(categoryId, includeChildCategories, 
                includeProducts);
            _logger.LogInformation($"Category with id {categoryId} fetched in CategoriesController");
            if(includeProducts && includeChildCategories)
            {
                _logger.LogInformation($"Returning category with id {categoryId} " +
                    $"including products & child categories.");
                return Ok(_mapper.Map<CategoryWithProductsAndCategoriesDto>(categoryEntity));
            }
            else if (includeProducts)
            {
                _logger.LogInformation($"Returning category with id {categoryId} " +
                   $"including products.");
                return Ok(_mapper.Map<CategoryWithProductsDto>(categoryEntity));
            }
            else if (includeChildCategories)
            {
                _logger.LogInformation($"Returning category with id {categoryId} " +
                   $"including child categories.");
                return Ok(_mapper.Map<CategoryWithChildCategoriesDto>(categoryEntity));
            }
            _logger.LogInformation($"Returning category with id {categoryId}.");
            return Ok(_mapper.Map<CategoryDto>(categoryEntity));
        }
        [HttpPost]
        [Authorize(Policy = "MustBeAdmin")]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CategoryForCreationDto category)
        {
            _logger.LogDebug($"Inside CreateCategory in CategoriesController.");

            var finalCategory = _mapper.Map<Entities.Category>(category);
            //check if given category name already exists
            if(await _vastraRepository
                .CategoryExistsWithNameAsync(finalCategory.CategoryName.Trim()))
            {
                throw new ItemWithNameAlreadyExistsException($"Category with name " +
                    $"{finalCategory.CategoryName} already exists.");
            }
            //add date added and date modified for new category
            finalCategory.DateAdded = DateTime.Now;
            finalCategory.DateModified = DateTime.Now;

            _logger.LogDebug($"finalCategory.DateAdded = {finalCategory.DateAdded} & " +
                $"finalCategory.DateModified = {finalCategory.DateModified}" +
                $" in CreateCategory() in CategoriesController.");
            await _vastraRepository.AddCategoryAsync(finalCategory);
            await _vastraRepository.SaveChangesAsync();

            var createdCategoryToReturn = _mapper.Map<Models.CategoryDto>(finalCategory);

            _logger.LogInformation($"Successfully returning created category " +
                $"with id {createdCategoryToReturn.CategoryId}.");
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
            _logger.LogDebug($"Inside UpdateCategory in CategoriesController.");

            if (!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                _logger.LogDebug($"Category with id {categoryId} was not found " +
                    $"in UpdateCategory() " +
                    $"in CategoriesController.");
                return NotFound();
            }
            var categoryEntity = await _vastraRepository.GetCategoryAsync(categoryId);
            if (categoryEntity == null)
            {
                _logger.LogDebug($"Category with id {categoryId} was not found " +
                    $"in UpdateCategory() " +
                    $"in CategoriesController.");
                return NotFound();
            }
            //if name is changed, check if category with changed name already exists
            if (!categoryEntity.CategoryName
                .Equals(category.CategoryName, StringComparison.OrdinalIgnoreCase))
            {
                if(await _vastraRepository
                    .CategoryExistsWithNameAsync(category.CategoryName.Trim()))
                {
                    throw new ItemWithNameAlreadyExistsException($"Category with name " +
                        $"{category.CategoryName} already exists.");
                }
            }
            _mapper.Map(category, categoryEntity);
            
            //update Modified Time of product
            categoryEntity.DateModified = DateTime.Now;

            _logger.LogDebug($"Updated categoryEntity.DateModified = {categoryEntity.DateModified} " +
                $"in UpdateCategory() " +
                    $"in CategoriesController.");
            await _vastraRepository.SaveChangesAsync();
            _logger.LogInformation($"Successfully updated category with id {categoryId}.");
            return NoContent();
        }
        [HttpPatch("{categoryId}")]
        [Authorize(Policy = "MustBeAdmin")]
        public async Task<ActionResult> PartiallyUpdateCategory(int categoryId,
            JsonPatchDocument<CategoryForUpdateDto> patchDocument)
        {
            _logger.LogDebug($"Inside PartiallyUpdateCategory in CategoriesController.");

            if (!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                _logger.LogDebug($"Category with id {categoryId} was not found " +
                    $"in PartiallyUpdateCategory() " +
                    $"in CategoriesController.");
                return NotFound();
            }
            var categoryEntity = await _vastraRepository.GetCategoryAsync(categoryId);
            if (categoryEntity == null)
            {
                _logger.LogDebug($"Category with id {categoryId} was not found " +
                    $"in PartiallyUpdateCategory() " +
                    $"in CategoriesController.");
                return NotFound();
            }
            var categoryToPatch = _mapper.Map<CategoryForUpdateDto>(categoryEntity);
            patchDocument.ApplyTo(categoryToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                _logger.LogDebug($"Validation failed " +
                    $"in PartiallyUpdateCategory() " +
                    $"in CategoriesController.");
                return BadRequest(ModelState);
            }
            if (!TryValidateModel(categoryToPatch))
            {
                _logger.LogDebug($"Validation failed " +
                    $"in PartiallyUpdateCategory() " +
                    $"in CategoriesController.");
                return BadRequest(ModelState);
            }
            //if name is changed, check if category with changed name already exists
            if (!categoryEntity.CategoryName
                .Equals(categoryToPatch.CategoryName, StringComparison.OrdinalIgnoreCase))
            {
                if (await _vastraRepository
                    .CategoryExistsWithNameAsync(categoryToPatch.CategoryName.Trim()))
                {
                    throw new ItemWithNameAlreadyExistsException($"Category with name " +
                        $"{categoryToPatch.CategoryName} already exists.");
                }
            }

            _mapper.Map(categoryToPatch, categoryEntity);
            //update Modified Time of category
            categoryEntity.DateModified = DateTime.Now;

            _logger.LogDebug($"Updated categoryEntity.DateModified = {categoryEntity.DateModified}" +
                $"in PartiallyUpdateCategory() " +
                    $"in CategoriesController.");
            await _vastraRepository.SaveChangesAsync();
            _logger.LogInformation($"Successfully updated category with id {categoryId}.");
            return NoContent();

        }
        [HttpDelete("{categoryId}")]
        [Authorize(Policy = "MustBeAdmin")]
        public async Task<ActionResult> DeleteCategory(int categoryId) {
            _logger.LogDebug($"Inside DeleteCategory in CategoriesController.");

            if (!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                _logger.LogDebug($"Category with id {categoryId} was not found " +
                    $"in DeleteCategory() " +
                    $"in CategoriesController.");
                return NotFound();
            }
            var categoryToDelete = await _vastraRepository.GetCategoryAsync(categoryId);
            _vastraRepository.DeleteCategory(categoryToDelete);
            await _vastraRepository.SaveChangesAsync();
            _logger.LogInformation($"Successfully deleted category with id {categoryId}.");
            return NoContent();
        }
        [HttpGet("{categoryId}/subCategories")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetChildCategoriesForCategory(int categoryId)
        {
            _logger.LogDebug($"Inside GetChildCategoriesForCategory in CategoriesController.");

            if (!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                _logger.LogDebug($"Category with id {categoryId} was not found " +
                    $"in GetChildCategoriesForCategory() " +
                    $"in CategoriesController.");
                return NotFound();
            }
            var childCategories = await _vastraRepository.GetChildCategoriesForCategoryAsync(categoryId);
            _logger.LogInformation($"Successfully fetched {childCategories.Count()} child categories " +
                $"for category with id {categoryId}.");
            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(childCategories));
        }
        //[HttpGet("{categoryId}/subCategories/{subCategoryId}")]
        //public async Task<ActionResult<CategoryDto>>  -- This is not needed currently as indivisual category can be fetched by GetCategory GET method

        [HttpPost("{categoryId}/subCategories")]
        [Authorize(Policy = "MustBeAdmin")]
        public async Task<ActionResult<CategoryDto>> CreateSubCategory(int categoryId, CategoryForCreationDto subCategory)
        {
            _logger.LogDebug($"Inside CreateSubCategory in CategoriesController.");

            if (!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                _logger.LogDebug($"Category with id {categoryId} was not found " +
                    $"in CreateSubCategory() " +
                    $"in CategoriesController.");
                return NotFound();
            }
            var finalSubcategory = _mapper.Map<Entities.Category>(subCategory);
            //set date added  & date modified for new sub category
            finalSubcategory.DateAdded = DateTime.Now;
            finalSubcategory.DateModified = DateTime.Now;

            _logger.LogDebug($"finalSubcategory.DateAdded = {finalSubcategory.DateAdded} & " +
                $"finalSubcategory.DateModified = {finalSubcategory.DateModified}" +
                $" in CreateSubCategory() in CategoriesController.");

            await _vastraRepository.AddChildCategoryForCategoryAsync(categoryId, finalSubcategory);
            await _vastraRepository.SaveChangesAsync();
            var createdSubCategoryToReturn = _mapper.Map<CategoryDto>(finalSubcategory);

            _logger.LogInformation($"Successfully returning created sub category with id " +
                $"{createdSubCategoryToReturn.CategoryId} in category with id {categoryId}.");
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
