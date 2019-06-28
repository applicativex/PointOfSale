using System;

namespace PointOfSale.Domain
{
    public struct ProductPrice : IEquatable<ProductPrice>
    {
        private ProductPrice(double value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            Value = value;
        }

        private double Value { get; }

        public static implicit operator double(ProductPrice value)
        {
            return value.Value;
        }

        public static implicit operator ProductPrice(double value)
        {
            return new ProductPrice(value);
        }

        public static implicit operator ProductPrice(int value)
        {
            return new ProductPrice(value);
        }

        public static ProductPrice operator +(ProductPrice left, ProductPrice right)
        {
            return left.Value + right.Value;
        }

        public bool Equals(ProductPrice other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ProductPrice && Equals((ProductPrice) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}