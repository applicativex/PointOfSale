using PointOfSale.Domain;

namespace PointOfSale.Api.Models
{
    public static class Converter
    {
        public static ProductResponseModel ToResponseModel(this Product value)
        {
            return new ProductResponseModel
            {
                Code = value.Code,
                Price = value.Price,
                SpecialOffer = value.SpecialOffer.ToResponseModel()
            };
        }

        public static SpecialOfferResponseModel ToResponseModel(this ISpecialOffer specialOffer)
        {
            return specialOffer is VolumedSpecialOffer volumedSpecialOffer
                ? new SpecialOfferResponseModel
                {
                    Amount = volumedSpecialOffer.Amount,
                    TotalPrice = volumedSpecialOffer.TotalPrice
                }
                : null;
        }
    }
}