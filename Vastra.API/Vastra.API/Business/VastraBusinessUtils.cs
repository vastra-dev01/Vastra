using Vastra.API.Entities;
using Vastra.API.Services;

namespace Vastra.API.Business
{
    public class VastraBusinessUtils : IVastraBusinessUtils
    {
        private readonly IVastraRepository _vastraRepository;
        private readonly ILogger<VastraBusinessUtils> _logger;
        private readonly IConfiguration _configuration;
        public VastraBusinessUtils(IVastraRepository vastraRepository,
            ILogger<VastraBusinessUtils> logger, IConfiguration configuration)
        {
            _vastraRepository = vastraRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> CategoryContainsCategoriesOrProducts(int categoryId)
        {
            _logger.LogDebug($"Inside CategoryContainsCategoriesOrProducts() " +
                $"in VastraBusinessUtils.");
            try
            {
                if (await _vastraRepository.CategoryContainsSubCategoriesOrProducts(categoryId))
                {
                    _logger.LogDebug($"Category with id {categoryId} contains " +
                        $"sub-categories or products.");
                    return true;
                }
                else
                {
                    _logger.LogDebug($"Category with id {categoryId} does not contain any " +
                        $"sub-categories or products.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return true;
            }
        }

        public async Task<bool> IsFirstAdmin(int userId)
        {
            _logger.LogDebug($"Inside IsFirstAdmin() in VastraBusinessUtils.");
            try
            {
                var user = await _vastraRepository.GetUserAsync(userId);
                if (user == null)
                {
                    _logger.LogDebug($"user with userId {userId} was not found in " +
                        $"IsFirstAdmin() in VastraBusinessUtils.");
                    return false;
                }
                if (user.PhoneNumber.Equals(_configuration["SampleUsers:AdminPhone"]))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return true;
            }
        }

        public async Task UpdateProductImage(string SKU, string path)
        {
            var product = await _vastraRepository.GetProductBySKUNumberAsync(SKU);
            if(product == null)
            {
                return;
            }
            product.Image = path;
            await _vastraRepository.SaveChangesAsync();
        }
    }
}
