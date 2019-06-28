using PointOfSale.Domain;
using Xunit;

namespace PointOfSale.Tests
{
    public class SpecialOfferTests
    {
        [Fact]
        public void EmptySpecialOfferShouldNotChangeTotalPrice()
        {
            var productsCount = 5;
            ProductPrice productPrice = 2.0;

            var sut = new EmptySpecialOffer();

            Assert.Equal((ProductPrice)(productPrice * productsCount), sut.Apply(productPrice, productsCount));
        }

        [Theory]

        [InlineData(1, 1.25, 3, 3, 1.25)]
        [InlineData(2, 1.25, 3, 3, 2.5)]
        [InlineData(3, 1.25, 3, 3, 3)]
        [InlineData(4, 1.25, 3, 3, 4.25)]
        [InlineData(5, 1.25, 3, 3, 5.5)]
        [InlineData(6, 1.25, 3, 3, 6)]

        [InlineData(1, 1, 6, 5, 1)]
        [InlineData(2, 1, 6, 5, 2)]
        [InlineData(3, 1, 6, 5, 3)]
        [InlineData(4, 1, 6, 5, 4)]
        [InlineData(5, 1, 6, 5, 5)]
        [InlineData(6, 1, 6, 5, 5)]
        [InlineData(7, 1, 6, 5, 6)]
        [InlineData(8, 1, 6, 5, 7)]
        public void VolumendSpecialOffer(int productsCount, double productPrice, int offerAmount, double offerPrice, double expectedTotalPrice)
        {
            var sut = new VolumedSpecialOffer(offerAmount, offerPrice);

            Assert.Equal((ProductPrice)expectedTotalPrice, sut.Apply(productPrice, productsCount));
        }
    }
}
