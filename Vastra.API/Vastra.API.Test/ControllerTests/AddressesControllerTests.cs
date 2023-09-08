using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Vastra.API.Controllers;
using Vastra.API.Entities;
using Vastra.API.Models;
using Vastra.API.Models.ForCreationAndUpdate;
using Vastra.API.Services;
using Vastra.API.Test.Fixtures;
using Xunit;

namespace Vastra.API.Test.ControllerTests
{
    [Collection("VastraServicesCollection")]
    public class AddressesControllerTests
    {
        //dependencies
        private readonly VastraServicesClassFixture _vastraServicesClassFixture;
        private readonly Mock<IVastraRepository> _vastraRepoMock;
        private readonly Mock<ILogger<AddressesController>> _loggerMock;
        private readonly IMapper _mapperMock;
        private readonly HttpContext _context;
        //addressesController
        private readonly AddressesController _addressesController;
        //data members
        private readonly Address _firstAddress;
        private readonly Address _secondAddress;
        public AddressesControllerTests(VastraServicesClassFixture vastraServicesClassFixture)
        {
            //initialize dependencies
            _vastraServicesClassFixture = vastraServicesClassFixture;
            _loggerMock = new Mock<ILogger<AddressesController>>();
            _mapperMock = _vastraServicesClassFixture.TestMapper;
            _vastraRepoMock = (Mock<IVastraRepository>)_vastraServicesClassFixture.VastraTestRepository;
            _context = _vastraServicesClassFixture.Context;

            //setup common methods
            _vastraRepoMock
                .Setup(m => m.RoleExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            _vastraRepoMock
                .Setup(m => m.UserExistsWithRoleAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            _vastraRepoMock
                .Setup(m => m.ValidateUserClaim(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            //initialize two addresses
            _firstAddress = new Address("Patna", "Patna", "Bihar", 800001, "India");
            _secondAddress = new Address("Pune", "Pune", "Maharashtra", 411038, "India");
            _vastraRepoMock
           .Setup(m => m.GetAddressesForUserAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
           .ReturnsAsync(

               (new List<Address>
               {
                _firstAddress, _secondAddress
               },
               new PaginationMetadata(2, 10, 1)
               {
                   TotalPageCount = 1
               })

            );
            //initialize address controller
            _addressesController = new AddressesController(_vastraRepoMock.Object,
                _mapperMock, _loggerMock.Object);
            _addressesController.ControllerContext = new ControllerContext()
            {
                HttpContext = _context
            };

        }
        [Fact]
        public async Task GetAddresses_GetAction_MustReturnOkObjectResultWithCorrectAddressDataAndNumberOfAddresses()
        {
            //Arrange
            
            //Act
            var result = await _addressesController.GetAddresses(1, 1);
            //Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<AddressDto>>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<AddressDto>>(okObjectResult.Value);
            Assert.Equal(2, dtos.Count());
            var firstAddress = dtos.First();
            Assert.Equal(_firstAddress.State, firstAddress.State);
            Assert.Equal(_firstAddress.Country, firstAddress.Country);
            Assert.Equal(_firstAddress.City, firstAddress.City);
            Assert.Equal(_firstAddress.Location, firstAddress.Location);
            Assert.Equal(_firstAddress.PinCode, firstAddress.PinCode);

        }

        [Fact]
        public async Task GetAddress_GetAction_MustReturnOkObjectResultWithCorrectAddressData()
        {
            //Arrange
            _vastraRepoMock
                .Setup(m => m.GetAddressForUserAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(
                    _firstAddress
                );
            //Act
            var result = await _addressesController.GetAddress(1, 1, 1);
            //Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var address = Assert.IsAssignableFrom<AddressDto>(okObjectResult.Value);
            Assert.Equal(_firstAddress.State, address.State);
            Assert.Equal(_firstAddress.Country, address.Country);
            Assert.Equal(_firstAddress.City, address.City);
            Assert.Equal(_firstAddress.Location, address.Location);
            Assert.Equal(_firstAddress.PinCode, address.PinCode);
        }
        [Fact]
        public async Task CreateAddress_PostAction_MustReturnCreatedAtRouteResultWithCorrectAddressData()
        {
            //Arrange
            _vastraRepoMock
                .Setup(m => m.AddAddressForUserAsync(It.IsAny<int>(), It.IsAny<Address>()));
            AddressForCreationDto address = new AddressForCreationDto()
            {
                City = _firstAddress.City,
                Location = _firstAddress.Location,
                PinCode = _firstAddress.PinCode,
                Country = _firstAddress.Country,
                State = _firstAddress.State,
                Tag = "Home",
                Type = 1
            };
            //Act
            var result = await _addressesController.CreateAddress(1, 1, address);
            //Assert
            var actionResult = Assert.IsType<ActionResult<AddressDto>>(result);
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(actionResult.Result);
            
            var routeName = createdAtRouteResult.RouteName;
            var routeValues = createdAtRouteResult.RouteValues;

            Assert.NotNull(routeValues);
            Assert.NotNull(createdAtRouteResult.Value);

            var createdAddress = (AddressDto)createdAtRouteResult.Value;

            Assert.Equal("GetAddress", routeName);
            Assert.Equal(1, routeValues["userId"]);
            Assert.Equal(1, routeValues["roleId"]);
            Assert.Equal(createdAddress.AddressId, routeValues["addressId"]);

            Assert.Equal(address.State, createdAddress.State);
            Assert.Equal(address.City, createdAddress.City);
            Assert.Equal(address.Location, createdAddress.Location);
            Assert.Equal(address.Country, createdAddress.Country);
            Assert.Equal(address.PinCode, createdAddress.PinCode);
            Assert.Equal(address.Tag, createdAddress.Tag);
            Assert.Equal(address.Type, createdAddress.Type);
        }
        [Fact]
        public async Task CreateAddress_PostAction_MustSetAddressAsPrimaryWhenNoAddressesPresent()
        {
            //Arrange
            _vastraRepoMock
                .Setup(m => m.GetAddressesForUserAsync(It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<int>()))
                .ReturnsAsync(
                    (new List<Address>
                   {
                        //returning empty List
                   },
                   new PaginationMetadata(2, 10, 1)
                   {
                       TotalPageCount = 1
                   })
                );

            _vastraRepoMock
                .Setup(m => m.AddAddressForUserAsync(It.IsAny<int>(), It.IsAny<Address>()));

            AddressForCreationDto address = new AddressForCreationDto()
            {
                City = _firstAddress.City,
                Location = _firstAddress.Location,
                PinCode = _firstAddress.PinCode,
                Country = _firstAddress.Country,
                State = _firstAddress.State,
                Tag = "Home",
                Type = 1
            };
            //Act
            var result = await _addressesController.CreateAddress(1, 1, address);

            //Assert
            var actionResult = Assert.IsType<ActionResult<AddressDto>>(result);
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(actionResult.Result);

            
            Assert.NotNull(createdAtRouteResult.Value);

            var createdAddress = (AddressDto)createdAtRouteResult.Value;

            
            Assert.Equal(0, createdAddress.Type);
        }
        

    }
}
