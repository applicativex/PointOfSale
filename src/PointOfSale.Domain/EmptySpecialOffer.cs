namespace PointOfSale.Domain
{
    public class EmptySpecialOffer : ISpecialOffer
    {
        public ProductPrice Apply(ProductPrice productPrice, int productCount)
        {
            return productPrice * productCount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return true;
        }

        public override int GetHashCode() => 0;
    }
}