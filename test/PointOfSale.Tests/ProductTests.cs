using PointOfSale.Domain;
using Xunit;

namespace PointOfSale.Tests
{
    public class ProductTests
    {
        [Fact]
        public void ByDefaultProductCreatedWithEmptyOffer()
        {
            var product = new Product("A", 2.25);
            Assert.True(product.SpecialOffer is EmptySpecialOffer);
        }
    }
}
