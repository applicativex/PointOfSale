namespace PointOfSale.Api.Models
{
    public class ProductResponseModel
    {
        public string Code { get; set; }
        public double Price { get; set; }
        public SpecialOfferResponseModel SpecialOffer { get; set; }
    }
}