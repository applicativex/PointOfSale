using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PointOfSale.Domain
{
    public interface IProductCatalog : IEnumerable<Product>
    {
        bool TryGetProduct(ProductCode productCode, out Product product);
    }

    public class ProductCatalog : IProductCatalog
    {
        private readonly Dictionary<ProductCode, Product> _products;

        public ProductCatalog(IEnumerable<Product> products)
        {
            _products = products.ToDictionary(x => x.Code, x => x);
        }

        public ProductCatalog(params Product[] products) : this(products as IEnumerable<Product>)
        {
        }

        public bool TryGetProduct(ProductCode productCode, out Product product)
        {
            return _products.TryGetValue(productCode, out product);
        }

        public IEnumerator<Product> GetEnumerator()
        {
            return _products.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}