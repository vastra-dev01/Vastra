using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Vastra.API.Models;
using Vastra.API.Models.ForCreationAndUpdate;
using Vastra.API.Services;

namespace Vastra.API.Controllers
{
    [ApiController]
    [Route("api/categories/{categoryId}/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IVastraRepository _vastraRepository;
        private readonly IMapper _mapper;
        const int maxProductsPageSize = 20;
        public ProductsController(IVastraRepository vastraRepository, IMapper mapper)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(int categoryId,
            string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > maxProductsPageSize)
            {
                pageSize = maxProductsPageSize;
            }
            var (productEntities, paginationMetadata) = await _vastraRepository.GetProductsForCategoryAsync(
                categoryId, name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            return Ok(_mapper.Map<IEnumerable<ProductDto>>(productEntities));
        }
        [HttpHead("{productId}")]
        [HttpGet("{productId}", Name = "GetProduct")]
        public async Task<IActionResult> GetProduct(int categoryId, int productId)
        {
            if(!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                return NotFound();
            }
            var product = await _vastraRepository.GetProductForCategoryAsync(categoryId, productId);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ProductDto>(product));
        }
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(int categoryId,
            ProductForCreationDto product)
        {
            var category = await _vastraRepository.GetCategoryAsync(categoryId);
            if (category == null)
            {
                return NotFound();
            }
            var finalProduct = _mapper.Map<Entities.Product>(product);

            // set date added and date modified for new product
            finalProduct.DateAdded = DateTime.Now;
            finalProduct.DateModified = DateTime.Now;

            await _vastraRepository.AddProductForCategoryAsync(categoryId, finalProduct);
            await _vastraRepository.SaveChangesAsync();

            var createdProductToReturn = _mapper.Map<Models.ProductDto>(finalProduct);
            return CreatedAtRoute("GetProduct",
            new
            {
                categoryId = categoryId,
                productId = createdProductToReturn.ProductId
            },
            createdProductToReturn
            );
        }
        [HttpPut("{productId}")]
        public async Task<ActionResult> UpdateProduct(int categoryId, int productId,
            ProductForUpdateDto product)
        {
            if(!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                return NotFound();
            }
            var productEntity = await _vastraRepository.GetProductForCategoryAsync(categoryId, productId);
            if(productEntity == null)
            {
                return NotFound();
            }
            _mapper.Map(product, productEntity);
            //update Modified Time of product
            productEntity.DateModified = DateTime.Now;
            await _vastraRepository.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{productId}")]
        public async Task<ActionResult> PartiallyUpdateProduct(int categoryId, int productId,
            JsonPatchDocument<ProductForUpdateDto> patchDocument)
        {
            if(!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                return NotFound();
            }
            var productEntity = await _vastraRepository.GetProductForCategoryAsync(categoryId, productId);
            if(productEntity == null)
            {
                return NotFound();
            }
            var productToPatch = _mapper.Map<ProductForUpdateDto>(productEntity);
            patchDocument.ApplyTo(productToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!TryValidateModel(productToPatch))
            {
                return BadRequest(ModelState);
            }
            _mapper.Map(productToPatch, productEntity);
            //update Modified Time of product
            productEntity.DateModified = DateTime.Now;
            await _vastraRepository.SaveChangesAsync();
            return NoContent();

        }

        [HttpDelete("{productId}")]
        public async Task<ActionResult> DeleteProduct(int categoryId, int productId)
        {
            if(!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                return NotFound();
            }
            var productToDelete = await _vastraRepository.GetProductForCategoryAsync(categoryId, productId);
            if(productToDelete == null)
            {
                return NotFound();
            }
            _vastraRepository.DeleteProduct(productToDelete);
            await _vastraRepository.SaveChangesAsync();
            return NoContent();
        }

    }
}
