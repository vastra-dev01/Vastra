using Microsoft.AspNetCore.Mvc;
using Vastra.API.Services;

namespace Vastra.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IVastraRepository _vastraRepository;
        public ProductsController(IVastraRepository vastraRepository)
        {
            _vastraRepository = vastraRepository;
        }

    }
}
