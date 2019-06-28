using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PointOfSale.Domain
{
    public class PointOfSaleTerminal : IEnumerable<Product>
    {
        private readonly ConcurrentQueue<Product> _products = new ConcurrentQueue<Product>();

        private IProductCatalog _productCatalog;

        public void SetPricing(IProductCatalog productCatalog)
        {
            _productCatalog = productCatalog;
        }

        public void Scan(string productCode)
        {
            if (_productCatalog == null)
            {
                throw new InvalidOperationException("Please set pricing before scanning products");
            }

            if (_productCatalog.TryGetProduct(productCode, out var product))
            {
                _products.Enqueue(product);
            }
            else
            {
                throw new ProductNotFoundException();
            }
        }

        public double CalculateTotal()
        {
            return _products.GroupBy(x =>
                    (productCode: x.Code, productPrice: x.Price, specialOffer: x.SpecialOffer))
                .Select(g => g.Key.specialOffer.Apply(g.Key.productPrice, g.Count()))
                .Aggregate(0.0, (ac, x) => ac + (double) x);
        }

        public IEnumerator<Product> GetEnumerator()
        {
            return _products.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}