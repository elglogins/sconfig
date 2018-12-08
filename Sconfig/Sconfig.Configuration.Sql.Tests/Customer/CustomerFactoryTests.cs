using Sconfig.Configuration.Sql.Factories;
using Sconfig.Interfaces.Factories;
using Xunit;

namespace Sconfig.Configuration.Sql.Tests
{
    public class CustomerFactoryTests
    {
        private readonly ICustomerFactory _customerFactory;

        public CustomerFactoryTests()
        {
            _customerFactory = new CustomerFactory();
        }

        [Fact]
        public void InitNewCustomerModel()
        {
            var result = _customerFactory.InitCustomerModel();
            Assert.NotNull(result);
        }
    }
}
