namespace PointOfSale.Api.Models
{
    public class ScannedProductResponseModel
    {
        public int ProductsCount { get; set; }
        public SpecialOfferResponseModel SpecialOffer { get; set; }
        public string Code { get; set; }
        public double Price { get; set; }
    }
}