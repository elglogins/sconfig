using System;
using System.Threading.Tasks;
using Moq;
using Sconfig.Contracts.Customer;
using Sconfig.Contracts.Customer.Enums;
using Sconfig.Contracts.Customer.Writes;
using Sconfig.Exceptions;
using Sconfig.Interfaces.Factories;
using Sconfig.Interfaces.Models;
using Sconfig.Interfaces.Repositories;
using Sconfig.Interfaces.Services;
using Sconfig.Services;
using Sconfig.Tests.Models;
using Xunit;

namespace Sconfig.Tests
{
    public class CustomerServiceTests
    {
        // services, factories
        private readonly ICustomerService _customerService;

        // repository entities
        private readonly ICustomerModel _storedCustomerModel;

        public CustomerServiceTests()
        {
            // repository entities
            _storedCustomerModel = new CustomerTestModel()
            {
                Active = true,
                CreatedOn = DateTime.Now,
                Id = "TEST-USER-1",
                Name = "TEST USER"
            };

            // services, factories
            var customerFactoryMock = new Mock<ICustomerFactory>();
            customerFactoryMock
               .Setup(_ => _.InitCustomerModel())
               .Returns(new CustomerTestModel());

            // readsH
            var customerRepositoryMock = new Mock<ICustomerRepository>();
            customerRepositoryMock
               .Setup(_ => _.Get(It.Is<string>(s => s == _storedCustomerModel.Id)))
               .Returns(Task.FromResult(_storedCustomerModel));

            customerRepositoryMock
             .Setup(_ => _.GetByName(It.Is<string>(s => s == _storedCustomerModel.Name)))
             .Returns(Task.FromResult(_storedCustomerModel));

            // writes
            customerRepositoryMock
               .Setup(_ => _.Insert(It.IsAny<ICustomerModel>()))
               .Returns<ICustomerModel>(x => Task.FromResult(x));

            customerRepositoryMock
               .Setup(_ => _.Save(It.IsAny<ICustomerModel>()))
               .Returns<ICustomerModel>(x => x);

            _customerService = new CustomerService(customerRepositoryMock.Object, customerFactoryMock.Object);
        }

        [Fact]
        public void GetExisting()
        {
            var result = _customerService.Get(_storedCustomerModel.Id).Result;

            Assert.NotNull(result);
            Assert.Equal(_storedCustomerModel.Id, result.Id);
            Assert.Equal(_storedCustomerModel.Name, result.Name);
        }

        [Fact]
        public void GetCustomerWithNullIdentifier()
        {
            var result = _customerService.Get(null).Result;
            Assert.Null(result);
        }

        [Fact]
        public void GetNotExisting()
        {
            var result = _customerService.Get("NOT-EXISTING-USER-ID").Result;
            Assert.Null(result);
        }

        [Theory]
        [InlineData("MY-TEST-CUSTOMER")]
        [InlineData("ThisIsEdgeValueOfAllowedLength")]
        public void Create(string name)
        {
            var contract = new CreateCustomerContract()
            {
                Name = name
            };

            var result = _customerService.Create(contract).Result;
            Assert.NotNull(result);
            Assert.NotNull(result.Id);
            Assert.Equal(contract.Name, result.Name);
            Assert.NotEqual(DateTime.MinValue, result.CreatedOn);
            Assert.True(result.Active);
            Assert.NotNull(result.Id);
        }

        [Fact]
        public async Task CreateAlreadyExisting()
        {
            var contract = new CreateCustomerContract()
            {
                Name = _storedCustomerModel.Name
            };

            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => _customerService.Create(contract));
            Assert.Equal(CustomerValidationCode.CUSTOMER_ALREADY_EXISTS.ToString(), exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ThisNameIsLongerThanIsAllowed!!")]
        public async Task CreateWithInvalidName(string name)
        {
            var contract = new CreateCustomerContract()
            {
                Name = name
            };

            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => _customerService.Create(contract));
            Assert.Equal(CustomerValidationCode.INVALID_CUSTOMER_NAME.ToString(), exception.Message);
        }

        [Fact]
        public async Task Disable()
        {
            var result = await _customerService.Disable(_storedCustomerModel.Id);
            Assert.NotNull(result);
            Assert.False(result.Active);
        }

        [Fact]
        public async Task DisableNotExisting()
        {
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => _customerService.Disable("NOT-EXISTING-CUSTOMER-ID"));
            Assert.Equal(CustomerValidationCode.CUSTOMER_DOES_NOT_EXIST.ToString(), exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task DisableWithInvalidId(string id)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _customerService.Disable(id));
        }

        [Fact]
        public async Task Enable()
        {
            var result = await _customerService.Enable(_storedCustomerModel.Id);
            Assert.NotNull(result);
            Assert.True(result.Active);
        }

        [Fact]
        public async Task EnableNotExisting()
        {
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => _customerService.Enable("NOT-EXISTING-CUSTOMER-ID"));
            Assert.Equal(CustomerValidationCode.CUSTOMER_DOES_NOT_EXIST.ToString(), exception.Message);
        }

        [Fact]
        public async Task Edit()
        {
            var editContract = new EditCustomerContract()
            {
                Id = _storedCustomerModel.Id,
                Name = "ThisIsNewAndValidName"
            };
            var result = await _customerService.Edit(editContract);

            Assert.NotNull(result);
            Assert.Equal(_storedCustomerModel.Name, result.Name);
        }

        [Fact]
        public async Task EditNotExisting()
        {
            var contract = new EditCustomerContract()
            {
                Id = "NOT-EXISTING-CUSTOMER-ID",
                Name = "ThisIsNewAndValidName"
            };

            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => _customerService.Edit(contract));
            Assert.Equal(CustomerValidationCode.CUSTOMER_DOES_NOT_EXIST.ToString(), exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ThisNameIsLongerThanIsAllowed!!")]
        public async Task EditWithInvalidName(string name)
        {
            var contract = new EditCustomerContract()
            {
                Name = name,
                Id = _storedCustomerModel.Id,
            };

            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => _customerService.Edit(contract));
            Assert.Equal(CustomerValidationCode.INVALID_CUSTOMER_NAME.ToString(), exception.Message);
        }
    }
}
