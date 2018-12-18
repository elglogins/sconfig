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
using Sconfig.Services;
using Sconfig.Tests.Models;
using Xunit;

namespace Sconfig.Tests
{
    public class CustomerServiceTests
    {
        #region Privates / mocking

        private ICustomerModel DefaultCustomerModel
        {
            get
            {
                return new CustomerTestModel()
                {
                    Active = true,
                    CreatedOn = DateTime.Now,
                    Id = "TEST-USER-1",
                    Name = "TEST USER"
                };
            }
        }

        private Mock<ICustomerFactory> DefaultCustomerFactoryMock
        {
            get
            {
                var customerFactoryMock = new Mock<ICustomerFactory>();
                customerFactoryMock
                   .Setup(_ => _.InitCustomerModel())
                   .Returns(new CustomerTestModel());
                return customerFactoryMock;
            }
        }

        private Mock<ICustomerRepository> DefaultCustomerRepositoryMock
        {
            get
            {
                var customerRepositoryMock = new Mock<ICustomerRepository>();
                customerRepositoryMock
                   .Setup(_ => _.Get(It.Is<string>(s => s == DefaultCustomerModel.Id)))
                   .Returns(Task.FromResult(DefaultCustomerModel));

                customerRepositoryMock
                 .Setup(_ => _.GetByName(It.Is<string>(s => s == DefaultCustomerModel.Name)))
                 .Returns(Task.FromResult(DefaultCustomerModel));

                // writes
                customerRepositoryMock
                   .Setup(_ => _.Insert(It.IsAny<ICustomerModel>()))
                   .Returns<ICustomerModel>(x => Task.FromResult(x));

                customerRepositoryMock
                   .Setup(_ => _.Save(It.IsAny<ICustomerModel>()))
                   .Returns<ICustomerModel>(x => x);
                return customerRepositoryMock;
            }
        }

        #endregion

        [Fact]
        public void GetExisting()
        {
            var customerService = new CustomerService(DefaultCustomerRepositoryMock.Object, DefaultCustomerFactoryMock.Object);
            var result = customerService.Get(DefaultCustomerModel.Id).Result;

            Assert.NotNull(result);
            Assert.Equal(DefaultCustomerModel.Id, result.Id);
            Assert.Equal(DefaultCustomerModel.Name, result.Name);
        }

        [Fact]
        public void GetCustomerWithNullIdentifier()
        {
            var customerService = new CustomerService(DefaultCustomerRepositoryMock.Object, DefaultCustomerFactoryMock.Object);
            var result = customerService.Get(null).Result;
            Assert.Null(result);
        }

        [Fact]
        public void GetNotExisting()
        {
            var customerService = new CustomerService(DefaultCustomerRepositoryMock.Object, DefaultCustomerFactoryMock.Object);
            var result = customerService.Get("NOT-EXISTING-USER-ID").Result;
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

            var customerRepositoryMock = DefaultCustomerRepositoryMock;
            var customerService = new CustomerService(customerRepositoryMock.Object, DefaultCustomerFactoryMock.Object);
            var result = customerService.Create(contract).Result;

            Assert.NotNull(result);
            Assert.NotNull(result.Id);
            Assert.Equal(contract.Name, result.Name);
            Assert.NotEqual(DateTime.MinValue, result.CreatedOn);
            Assert.True(result.Active);

            customerRepositoryMock.Verify(m => m.Insert(It.IsAny<ICustomerModel>()), Times.Once);
        }

        [Fact]
        public async Task CreateAlreadyExisting()
        {
            var contract = new CreateCustomerContract()
            {
                Name = DefaultCustomerModel.Name
            };

            var customerService = new CustomerService(DefaultCustomerRepositoryMock.Object, DefaultCustomerFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => customerService.Create(contract));
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

            var customerService = new CustomerService(DefaultCustomerRepositoryMock.Object, DefaultCustomerFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => customerService.Create(contract));
            Assert.Equal(CustomerValidationCode.INVALID_CUSTOMER_NAME.ToString(), exception.Message);
        }

        [Fact]
        public async Task Disable()
        {
            var customerRepositoryMock = DefaultCustomerRepositoryMock;
            var customerService = new CustomerService(customerRepositoryMock.Object, DefaultCustomerFactoryMock.Object);
            var result = await customerService.Disable(DefaultCustomerModel.Id);
            Assert.NotNull(result);
            Assert.False(result.Active);
            customerRepositoryMock.Verify(m => m.Save(It.Is<ICustomerModel>(c => c.Id == DefaultCustomerModel.Id)), Times.Once);
        }

        [Fact]
        public async Task DisableNotExisting()
        {
            var customerService = new CustomerService(DefaultCustomerRepositoryMock.Object, DefaultCustomerFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => customerService.Disable("NOT-EXISTING-CUSTOMER-ID"));
            Assert.Equal(CustomerValidationCode.CUSTOMER_DOES_NOT_EXIST.ToString(), exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task DisableWithInvalidId(string id)
        {
            var customerService = new CustomerService(DefaultCustomerRepositoryMock.Object, DefaultCustomerFactoryMock.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => customerService.Disable(id));
        }

        [Fact]
        public async Task Enable()
        {
            var customerRepositoryMock = DefaultCustomerRepositoryMock;
            var customerService = new CustomerService(customerRepositoryMock.Object, DefaultCustomerFactoryMock.Object);
            var result = await customerService.Enable(DefaultCustomerModel.Id);
            Assert.NotNull(result);
            Assert.True(result.Active);
            customerRepositoryMock.Verify(m => m.Save(It.Is<ICustomerModel>(c => c.Id == DefaultCustomerModel.Id)), Times.Once);
        }

        [Fact]
        public async Task EnableNotExisting()
        {
            var customerService = new CustomerService(DefaultCustomerRepositoryMock.Object, DefaultCustomerFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => customerService.Enable("NOT-EXISTING-CUSTOMER-ID"));
            Assert.Equal(CustomerValidationCode.CUSTOMER_DOES_NOT_EXIST.ToString(), exception.Message);
        }

        [Fact]
        public async Task Edit()
        {
            var editContract = new EditCustomerContract()
            {
                Id = DefaultCustomerModel.Id,
                Name = "ThisIsNewAndValidName"
            };

            var customerRepositoryMock = DefaultCustomerRepositoryMock;
            var customerService = new CustomerService(customerRepositoryMock.Object, DefaultCustomerFactoryMock.Object);
            var result = await customerService.Edit(editContract);

            Assert.NotNull(result);
            Assert.Equal(editContract.Name, result.Name);
            customerRepositoryMock.Verify(m => m.Save(It.Is<ICustomerModel>(c => c.Id == editContract.Id)), Times.Once);
        }

        [Fact]
        public async Task EditNotExisting()
        {
            var contract = new EditCustomerContract()
            {
                Id = "NOT-EXISTING-CUSTOMER-ID",
                Name = "ThisIsNewAndValidName"
            };

            var customerService = new CustomerService(DefaultCustomerRepositoryMock.Object, DefaultCustomerFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => customerService.Edit(contract));
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
                Id = DefaultCustomerModel.Id,
            };

            var customerService = new CustomerService(DefaultCustomerRepositoryMock.Object, DefaultCustomerFactoryMock.Object);
            var exception = await Assert.ThrowsAsync<ValidationCodeException>(() => customerService.Edit(contract));
            Assert.Equal(CustomerValidationCode.INVALID_CUSTOMER_NAME.ToString(), exception.Message);
        }
    }
}
