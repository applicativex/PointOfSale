using System;
using PointOfSale.Domain;
using Xunit;

namespace PointOfSale.Tests
{
    public class ProductCodeTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void ProductCodeShouldNotBeEmptyStringOrNull(string productCodeValue)
        {
            Assert.Throws<ArgumentNullException>(() => (ProductCode) productCodeValue);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("B")]
        public void RegularLettersAreValidProductCodes(string productCodeValue)
        {
            ProductCode productCode = productCodeValue;
            Assert.Equal(productCodeValue, productCode.Value);
        }
    }
}
