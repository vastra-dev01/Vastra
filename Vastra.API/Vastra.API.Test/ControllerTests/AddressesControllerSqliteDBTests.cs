using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vastra.API.Controllers;
using Vastra.API.Entities;
using Vastra.API.Models.ForCreationAndUpdate;
using Vastra.API.Services;
using Vastra.API.Test.Fixtures;

namespace Vastra.API.Test.ControllerTests
{
    [Collection("VastraServicesCollection")]
    public class AddressesControllerSqliteDBTests
    {
        //dependencies
        private readonly VastraServicesClassFixture _vastraServicesClassFixture;
        private readonly VastraTestDBClassFixture _vastraTestDBClassFixture;
        private readonly VastraRepository _vastraRepo;
        private readonly Mock<ILogger<AddressesController>> _loggerMock;
        private readonly IMapper _mapperMock;
        private readonly HttpContext _context;
        //addressesController
        private readonly AddressesController _addressesController;
        //usersController
        private readonly UsersController _usersController;
        //data members
        private readonly Address _firstAddress;
        private readonly Address _secondAddress;
        public AddressesControllerSqliteDBTests(VastraServicesClassFixture vastraServicesClassFixture,
            VastraTestDBClassFixture vastraTestDBClassFixture)
        {
            //initialize dependencies
            _vastraServicesClassFixture = vastraServicesClassFixture;
            _vastraTestDBClassFixture = vastraTestDBClassFixture;
            _loggerMock = new Mock<ILogger<AddressesController>>();
            _mapperMock = _vastraServicesClassFixture.TestMapper;
            _context = _vastraServicesClassFixture.Context;
            _vastraRepo = _vastraTestDBClassFixture.VastraRepository;

            //initialize two addresses
            _firstAddress = new Address("Patna", "Patna", "Bihar", 800001, "India");
            _secondAddress = new Address("Pune", "Pune", "Maharashtra", 411038, "India");

            
            //initialize address controller
            _addressesController = new AddressesController(_vastraRepo,
                _mapperMock, _loggerMock.Object);
            _addressesController.ControllerContext = new ControllerContext()
            {
                HttpContext = _context
            };
            //initialize users controller
            _usersController = new UsersController(_vastraRepo,
                _mapperMock, new Mock<ILogger<UsersController>>().Object);
            _addressesController.ControllerContext = new ControllerContext()
            {
                HttpContext = _context
            };
        }
        [Fact]
        public async Task CreateAddress_PostAction_MustSetOtherAddressesAsSecondaryWhenAddressCreatedIsPrimary()
        {
            //Arrange
            //setup the repo with user data
            var result = await _usersController.CreateUser(2, new UserForCreationDto()
            {
                FirstName = "test",
                LastName = "test",
                EmailId = "test@test.com",
                Password = "test@123",
                PhoneNumber = "1234567890",
            });
            var userCreated = result.Value;
            var userCreatedId = userCreated.UserId;
            //Act
            //add address 1
            var addressResult1 = await _addressesController.CreateAddress(2, userCreatedId, new AddressForCreationDto()
            {
                City = _firstAddress.City,
                Country = _firstAddress.Country,
                PinCode = _firstAddress.PinCode,
                State   = _firstAddress.State,
                Location = _firstAddress.Location,
                Tag = _firstAddress.Tag,
                Type = 0,
            });
            var addressResult1Id = addressResult1.Value.AddressId;
            //add address 2
            var addressResult2 = await _addressesController.CreateAddress(2, userCreatedId, new AddressForCreationDto()
            {
                City = _secondAddress.City,
                Country = _secondAddress.Country,
                PinCode = _secondAddress.PinCode,
                State = _secondAddress.State,
                Location = _secondAddress.Location,
                Tag = _secondAddress.Tag,
                Type = 1,
            });
            var addressResult2Id = addressResult2.Value.AddressId;
            //add a primary address
            var addressResultPrimary = await _addressesController.CreateAddress(2, userCreatedId, new AddressForCreationDto()
            {
                City = _secondAddress.City,
                Country = _secondAddress.Country,
                PinCode = _secondAddress.PinCode,
                State = _secondAddress.State,
                Location = _secondAddress.Location,
                Tag = _secondAddress.Tag,
                Type = 0, //0 for primary address
            });
            var addressResultPrimaryId = addressResultPrimary.Value.AddressId;

            //Assert
            var address1 = await _vastraRepo.GetAddressForUserAsync(userCreatedId, addressResult1Id);
            var address2 = await _vastraRepo.GetAddressForUserAsync(userCreatedId, addressResult2Id);
            var primaryAddress = await _vastraRepo.GetAddressForUserAsync(userCreatedId, addressResultPrimaryId);

            //address1 & address2 must be set as secondary
            Assert.Equal(1, address1.Type);
            Assert.Equal(1, address2.Type);

            Assert.Equal(0, primaryAddress.Type);
        }
    }
}
