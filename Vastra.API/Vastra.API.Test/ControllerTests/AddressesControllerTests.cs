using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Vastra.API.Controllers;
using Vastra.API.Entities;
using Vastra.API.Models;
using Vastra.API.Services;
using Xunit;

namespace Vastra.API.Test.ControllerTests
{
    public class AddressesControllerTests
    {
        [Fact]
        public async Task GetAddresses_GetAction_MustReturnOkObjectResult()
        {
            //Arrange
            var vastraRepositoryMock = new Mock<IVastraRepository>();
            vastraRepositoryMock
                .Setup(m => m.GetAddressesForUserAsync(It.IsAny<int>(), 1, 10))
                .ReturnsAsync(
                
                  (new List<Address>
                  {
                      new Address("Patna", "Patna", "Bihar", 800001, "India"),
                      new Address("Pune", "Pune", "Maharashtra", 411038, "India")

                  },
                  new PaginationMetadata(2, 10, 1)
                  {
                      TotalPageCount = 1
                  })
               
                );
            vastraRepositoryMock
                .Setup(m => m.RoleExistsAsync(It.IsAny<int>()))
                .ReturnsAsync (true);
            vastraRepositoryMock
                .Setup(m => m.UserExistsWithRoleAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync (true);
            vastraRepositoryMock
                .Setup(m => m.ValidateUserClaim(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            var loggerMock = new Mock<ILogger<AddressesController>>();
                
            var autoMapper = new Mock<IMapper>();
            

            var addressesController = new AddressesController(vastraRepositoryMock.Object,
                autoMapper.Object, loggerMock.Object);
            //Act
            var result = await addressesController.GetAddresses(1, 1);
            //Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<AddressDto>>>(result);
            Assert.IsType<OkObjectResult>(actionResult.Result);
        }
    }
}
