using System;

namespace PointOfSale.Domain
{
    public class ProductCode : IEquatable<ProductCode>
    {
        private ProductCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException();
            }

            Value = code;
        }

        public string Value { get; }

        public static implicit operator string(ProductCode value)
        {
            return value.Value;
        }

        public static implicit operator ProductCode(string value)
        {
            return new ProductCode(value);
        }

        public bool Equals(ProductCode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProductCode) obj);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }
    }
}