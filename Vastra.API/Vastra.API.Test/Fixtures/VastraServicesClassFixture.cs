using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Vastra.API.Controllers;
using Vastra.API.Profiles;
using Vastra.API.Services;

namespace Vastra.API.Test.Fixtures
{
    
    public class VastraServicesClassFixture : IDisposable
    {
        public Mock VastraTestRepository { get; }
        public Mock? TestLogger { get; }

        public IMapper TestMapper { get; }

        public HttpContext Context { get; }

        public VastraServicesClassFixture()
        {
            VastraTestRepository = new Mock<IVastraRepository>();
            TestLogger = null;

            //set default http context
            var userClaims = new List<Claim>()
            {
                new Claim("first_name", "test"),
                new Claim("last_name", "test"),
                new Claim("phone", "9999999999"),
                new Claim("email", "test@test.com"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var claimsIdentity = new ClaimsIdentity(userClaims,"UnitTest");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var httpContext = new DefaultHttpContext()
            {
                User = claimsPrincipal
            };
            Context = httpContext;
            //add mapping profiles to mapper
            var mapperConfig = new MapperConfiguration(
                (cfg) => 
                {
                    cfg.AddProfile<AddressProfile>();
                    cfg.AddProfile<CartItemProfile>();
                    cfg.AddProfile<CategoryProfile>();
                    cfg.AddProfile<OrderProfile>();
                    cfg.AddProfile<ProductProfile>();
                    cfg.AddProfile<RoleProfile>();
                    cfg.AddProfile<UserProfile>();
                }
             );
            TestMapper = new Mapper(mapperConfig);
             
        }

        public void Dispose()
        {
            //dispose any resource if needed
        }
    }
}
