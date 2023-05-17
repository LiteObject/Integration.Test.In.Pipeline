using Xunit.Abstractions;

namespace Demo.Product.Test
{
    public class UnitTests
    {
        private ITestOutputHelper _output;

        public UnitTests(ITestOutputHelper output) 
        { 
            _output = output;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public void Set_UnitPrice_Throws_Exception_On_Invalid_Values(decimal unitPrice)
        {
            // ARRANGE
            Api.Domain.Product product = new("Test Product", 1.99m);

            // ACT
            Action act = () => product.SetUnitPruce(unitPrice);

            // ASSERT
            Assert.Throws<ArgumentOutOfRangeException>(() => act());
        }
    }
}
