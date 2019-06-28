using System;
using PointOfSale.Domain;
using Xunit;

namespace PointOfSale.Tests
{
    public class ProductPriceTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void PriceShouldBeGreaterThanZero(double priceValue)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => (ProductPrice)priceValue);
        }

        [Fact]
        public void GreaterThanZeroValuesAreValidPrices()
        {
            var priceValue = 7.25;
            var price = (ProductPrice) priceValue;
            Assert.Equal(priceValue, (double)price);
        }
    }
}
