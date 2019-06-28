using System;
using PointOfSale.Domain;
using Xunit;

namespace PointOfSale.Tests
{
    public class PointOfSaleTerminalTests
    {
        private readonly PointOfSaleTerminal _sut;

        public PointOfSaleTerminalTests()
        {
            var productCatalog = new ProductCatalog(
                new Product("A", 1.25, new VolumedSpecialOffer(3, (ProductPrice) 3)),
                new Product("B", 4.25),
                new Product("C", 1, new VolumedSpecialOffer(6, (ProductPrice) 5)),
                new Product("D", 0.75));
            _sut = new PointOfSale.Domain.PointOfSaleTerminal();
            _sut.SetPricing(productCatalog);
        }

        [Fact]
        public void TotalShouldBeGreaterThanZeroAfterProductScan()
        {
            _sut.Scan("A");

            Assert.True(_sut.CalculateTotal() > 0);
        }

        [Theory]
        [InlineData("ABCDABA", 13.25)]
        [InlineData("CCCCCCC", 6)]
        [InlineData("ABCD", 7.25)]
        public void CheckGivenScanSequences(string products, double expectedTotalPrice)
        {
            foreach (var product in products)
            {
                _sut.Scan(product.ToString());
            }

            Assert.Equal(expectedTotalPrice, _sut.CalculateTotal());
        }

        [Fact]
        public void ShouldRaiseExceptionIfPricingNotSet()
        {
            var sut = new PointOfSaleTerminal();

            Assert.Throws<InvalidOperationException>(() => sut.Scan("A"));
        }

        [Fact]
        public void ShouldRaiseExceptionIfScanUnknownProduct()
        {
            var sut = new PointOfSaleTerminal();
            sut.SetPricing(new ProductCatalog());

            Assert.Throws<ProductNotFoundException>(() => sut.Scan("A"));
        }
    }
}
