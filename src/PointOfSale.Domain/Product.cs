using System;

namespace PointOfSale.Domain
{
    public class Product : IEquatable<Product>
    {
        public ProductCode Code { get; }
        public ProductPrice Price { get; private set; }
        public ISpecialOffer SpecialOffer { get; private set; }

        public Product(ProductCode code, ProductPrice price) : this(code, price, new EmptySpecialOffer())
        {
        }

        public Product(ProductCode code, ProductPrice price, ISpecialOffer specialOffer)
        {
            Code = code;
            Price = price;
            SpecialOffer = specialOffer;
        }

        public void ChangePrice(ProductPrice price)
        {
            Price = price;
        }

        public void SetSpecialOffer(ISpecialOffer specialOffer)
        {
            SpecialOffer = specialOffer;
        }

        public bool Equals(Product other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Code, other.Code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Product) obj);
        }

        public override int GetHashCode()
        {
            return (Code != null ? Code.GetHashCode() : 0);
        }
    }
}