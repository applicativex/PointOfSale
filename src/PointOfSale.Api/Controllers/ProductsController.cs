using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Api.Models;
using PointOfSale.Domain;

namespace PointOfSale.Api.Controllers
{
    [Route("api/products")]
    public class ProductsController : Controller
    {
        private readonly IProductCatalog _productCatalog;

        public ProductsController(IProductCatalog productCatalog)
        {
            _productCatalog = productCatalog;
        }

        /// <summary>
        /// Returns product by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("{code}")]
        public IActionResult GetProduct(string code)
        {
            return _productCatalog.TryGetProduct(code, out Product product)
                ? (IActionResult) Ok(product.ToResponseModel())
                : NotFound();
        }

        /// <summary>
        /// Returns list of all products in catalog
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(_productCatalog.Select(x=>x.ToResponseModel()).ToArray());
        }
    }
}