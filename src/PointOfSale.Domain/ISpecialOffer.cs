namespace PointOfSale.Domain
{
    public interface ISpecialOffer
    {
        ProductPrice Apply(ProductPrice productPrice, int productCount);
    }
}