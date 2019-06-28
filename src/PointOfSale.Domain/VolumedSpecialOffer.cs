using System;

namespace PointOfSale.Domain
{
    public class VolumedSpecialOffer : ISpecialOffer, IEquatable<VolumedSpecialOffer>
    {
        public VolumedSpecialOffer(int amount, ProductPrice totalPrice)
        {
            Amount = amount;
            TotalPrice = totalPrice;
        }

        public int Amount { get; }
        public ProductPrice TotalPrice { get; }

        public ProductPrice Apply(ProductPrice productPrice, int productCount)
        {
            var specialOffersCount = productCount / Amount;
            var priceWithoutDiscount = productCount * productPrice;
            var perOfferDiscount = Amount * productPrice - TotalPrice;
            return priceWithoutDiscount - (specialOffersCount > 0 ? specialOffersCount * perOfferDiscount : 0);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VolumedSpecialOffer) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Amount * 397) ^ TotalPrice.GetHashCode();
            }
        }

        public bool Equals(VolumedSpecialOffer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Amount == other.Amount && TotalPrice.Equals(other.TotalPrice);
        }
    }
}