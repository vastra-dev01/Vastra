using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Vastra.API.Models;
using Vastra.API.Models.CustomException;
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
        private readonly ILogger<ProductsController> _logger;
        private readonly string uploadPath;
        private readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
        const int maxProductsPageSize = 20;
        public ProductsController(IVastraRepository vastraRepository, IMapper mapper,
            ILogger<ProductsController> logger)
        {
            _vastraRepository = vastraRepository;
            _mapper = mapper;
            _logger = logger;
            uploadPath = Path.Combine(AppContext.BaseDirectory,"images");
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(int categoryId,
            string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogDebug($"Inside GetProducts in ProductsController.");
            if (!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                _logger.LogDebug($"Category with id {categoryId} was not found " +
                    $"in GetProducts() " +
                    $"in ProductsController.");
                return NotFound();
            }
            if (pageSize > maxProductsPageSize)
            {
                pageSize = maxProductsPageSize;
            }
            var (productEntities, paginationMetadata) = await _vastraRepository
                .GetProductsForCategoryAsync(
                categoryId, name, searchQuery, pageNumber, pageSize);

            _logger.LogInformation($"Total {productEntities.Count()} products fetched in ProductsController.");

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            return Ok(_mapper.Map<IEnumerable<ProductDto>>(productEntities));
        }
        [HttpHead("{productId}")]
        [HttpGet("{productId}", Name = "GetProduct")]
        public async Task<IActionResult> GetProduct(int categoryId, int productId)
        {
            _logger.LogDebug($"Inside GetProduct in ProductsController.");

            if (!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                _logger.LogDebug($"Category with id {categoryId} was not found " +
                    $"in GetProduct() " +
                    $"in ProductsController.");
                return NotFound();
            }
            var product = await _vastraRepository.GetProductForCategoryAsync(categoryId, productId);
            if (product == null)
            {
                _logger.LogDebug($"Product with id {productId} was not found " +
                    $"in GetProduct() " +
                    $"in ProductsController.");
                return NotFound();
            }

            _logger.LogInformation($"Successfully returning product with id {productId} " +
                $"in ProductsController.");
            return Ok(_mapper.Map<ProductDto>(product));
        }
        [Authorize(Policy = "MustBeAdmin")]
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(int categoryId,[FromForm]IFormFile imageFile,
            [FromForm]ProductForCreationDto product)
        {
            _logger.LogDebug($"Inside CreateProduct in ProductsController.");

            var category = await _vastraRepository.GetCategoryAsync(categoryId);
            if (category == null)
            {
                _logger.LogDebug($"Category with id {categoryId} was not found " +
                   $"in CreateProduct() " +
                   $"in ProductsController.");
                return NotFound();
            }
            //check SKU number
            if(await _vastraRepository.ProductWithSKUNumberExistsAsync(product.SKU))
            {
                throw new ProductWithSKUNumberAlreadyExistsException($"Product with SKU " +
                    $"{product.SKU} already exists.");
            }
            var finalProduct = _mapper.Map<Entities.Product>(product);

            //save image in images directory and save image path in db
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("No image file or empty file provided.");
            }

            // Check if the file extension is allowed
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid file format. Allowed formats: jpg, jpeg, png.");
            }

            // Generate a unique filename with SKU
            string uniqueFileName = finalProduct.SKU + fileExtension;

            string filePath = Path.Combine(uploadPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }


            // set date added and date modified for new product
            finalProduct.DateAdded = DateTime.Now;
            finalProduct.DateModified = DateTime.Now;

            //set image path
            finalProduct.Image = filePath;

            _logger.LogDebug($"Updated finalProduct.DateAdded = {finalProduct.DateAdded} " +
                $"& finalProduct.DateModified = {finalProduct.DateModified} " +
                $"& finalProduct.Image = {finalProduct.Image}"+
                $"in CreateProduct() " +
                $"in ProductsController.");

            await _vastraRepository.AddProductForCategoryAsync(categoryId, finalProduct);
            await _vastraRepository.SaveChangesAsync();

            var createdProductToReturn = _mapper.Map<Models.ProductDto>(finalProduct);

            _logger.LogInformation($"Successfully returnning created product with id " +
                $"{createdProductToReturn.ProductId}.");

            return CreatedAtRoute("GetProduct",
            new
            {
                categoryId = categoryId,
                productId = createdProductToReturn.ProductId
            },
            createdProductToReturn
            );
        }
        [Authorize(Policy = "MustBeAdmin")]
        [HttpPut("{productId}")]
        public async Task<ActionResult> UpdateProduct(int categoryId, int productId,
            ProductForUpdateDto product)
        {
            _logger.LogDebug($"Inside UpdateProduct in ProductsController.");

            if (!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                _logger.LogDebug($"Category with id {categoryId} was not found " +
                    $"in UpdateProduct() " +
                    $"in ProductsController.");
                return NotFound();
            }
            var productEntity = await _vastraRepository.GetProductForCategoryAsync(categoryId, productId);
            if(productEntity == null)
            {
                _logger.LogDebug($"Product with id {productId} was not found " +
                    $"in UpdateProduct() " +
                    $"in ProductsController.");
                return NotFound();
            }
            _mapper.Map(product, productEntity);
            //update Modified Time of product
            productEntity.DateModified = DateTime.Now;
            _logger.LogDebug($"Updated productEntity.DateAdded = {productEntity.DateAdded} " +
                $"in UpdateProduct() " +
                $"in ProductsController.");
            await _vastraRepository.SaveChangesAsync();

            _logger.LogInformation($"Successfully updated product with id {productId}.");

            return NoContent();
        }
        [Authorize(Policy = "MustBeAdmin")]
        [HttpPatch("{productId}")]
        public async Task<ActionResult> PartiallyUpdateProduct(int categoryId, int productId,
            JsonPatchDocument<ProductForUpdateDto> patchDocument)
        {
            _logger.LogDebug($"Inside PartiallyUpdateProduct in ProductsController.");

            if (!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                _logger.LogDebug($"Category with id {categoryId} was not found " +
                    $"in PartiallyUpdateProduct() " +
                    $"in ProductsController.");
                return NotFound();
            }
            var productEntity = await _vastraRepository.GetProductForCategoryAsync(categoryId, productId);
            if(productEntity == null)
            {
                _logger.LogDebug($"Product with id {productId} was not found " +
                    $"in PartiallyUpdateProduct() " +
                    $"in ProductsController.");
                return NotFound();
            }
            var productToPatch = _mapper.Map<ProductForUpdateDto>(productEntity);
            patchDocument.ApplyTo(productToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                _logger.LogDebug($"Validation failed in " +
                    $"in PartiallyUpdateProduct() " +
                    $"in ProductsController.");
                return BadRequest(ModelState);
            }
            if (!TryValidateModel(productToPatch))
            {
                _logger.LogDebug($"Validation failed in " +
                    $"in PartiallyUpdateProduct() " +
                    $"in ProductsController.");
                return BadRequest(ModelState);
            }
            _mapper.Map(productToPatch, productEntity);
            //update Modified Time of product
            productEntity.DateModified = DateTime.Now;
            _logger.LogDebug($"Updated productEntity.DateAdded = {productEntity.DateAdded} " +
                $"in PartiallyUpdateProduct() " +
                $"in ProductsController.");
            await _vastraRepository.SaveChangesAsync();
            _logger.LogInformation($"Successfully updated product with id {productId}.");
            return NoContent();

        }

        [Authorize(Policy = "MustBeAdmin")]
        [HttpDelete("{productId}")]
        public async Task<ActionResult> DeleteProduct(int categoryId, int productId)
        {
            _logger.LogDebug($"Inside DeleteProduct in ProductsController.");

            if (!await _vastraRepository.CategoryExistsAsync(categoryId))
            {
                _logger.LogDebug($"Category with id {categoryId} was not found " +
                    $"in DeleteProduct() " +
                    $"in ProductsController.");
                return NotFound();
            }
            var productToDelete = await _vastraRepository.GetProductForCategoryAsync(categoryId, productId);
            if(productToDelete == null)
            {
                _logger.LogDebug($"Product with id {productId} was not found " +
                    $"in DeleteProduct() " +
                    $"in ProductsController.");
                return NotFound();
            }
            _vastraRepository.DeleteProduct(productToDelete);
            await _vastraRepository.SaveChangesAsync();
            _logger.LogInformation($"Product with id {productId} deleted successfully");
            return NoContent();
        }

    }
}
