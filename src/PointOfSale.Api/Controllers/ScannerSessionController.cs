using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using PointOfSale.Api.Models;
using PointOfSale.Domain;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PointOfSale.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/scanner-session")]
    public class ScannerSessionController : Controller
    {
        private readonly IProductCatalog _productCatalog;

        private static readonly Dictionary<int, PointOfSaleTerminal> ScannerSessionsMap =
            new Dictionary<int, PointOfSaleTerminal>();

        private static int _cartIdGenerator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productCatalog"></param>
        public ScannerSessionController(IProductCatalog productCatalog)
        {
            _productCatalog = productCatalog;
        }

        // GET api/scanner-session/5
        /// <summary>
        /// Return current state of scanner session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetScannerSession")]
        public IActionResult GetScannerSession(int id)
        {
            if (!ScannerSessionsMap.TryGetValue(id, out var scannerSession)) return NotFound();
            var products = scannerSession
                .GroupBy(x => (productCode: x.Code, productPrice: x.Price, specialOffer: x.SpecialOffer))
                .Select(g => new ScannedProductResponseModel
                {
                    Code = g.Key.productCode,
                    Price = g.Key.productPrice,
                    ProductsCount = g.Count(),
                    SpecialOffer = g.Key.specialOffer.ToResponseModel()
                }).ToArray();
            return Ok(new ScannerSessionResponseModel
            {
                ScannedProducts = products
            });
        }

        // POST api/scanner-session
        /// <summary>
        /// Creates new scanner session
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateScannerSession()
        {
            var terminalId = Interlocked.Increment(ref _cartIdGenerator);
            var pointOfSaleTerminal = new PointOfSaleTerminal();
            pointOfSaleTerminal.SetPricing(_productCatalog);
            ScannerSessionsMap[terminalId] = pointOfSaleTerminal;
            return CreatedAtRoute("GetScannerSession", new { id = terminalId }, terminalId);
        }

        // POST api/scanner-session/1/products/A
        /// <summary>
        /// Scans product and add it to scanner session
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productCode"></param>
        /// <returns></returns>
        [HttpPost("{id}/products/{productCode}")]
        public IActionResult ScanProduct(int id, string productCode)
        {
            try
            {
                if (!ScannerSessionsMap.TryGetValue(id, out var scannerSession)) return NotFound();
                scannerSession.Scan(productCode);
            }
            catch (ProductNotFoundException e)
            {
                return NotFound();
            }
             
            return Ok();
        }

        // GET api/scanner-session/5/total
        /// <summary>
        /// Calculates total for scanner session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/total")]
        public IActionResult GetTotalPrice(int id)
        {
            if (!ScannerSessionsMap.TryGetValue(id, out var scannerSession)) return NotFound();
            return Ok(scannerSession.CalculateTotal());
        }

        // DELETE api/scanner-session/5
        /// <summary>
        /// Removes scanner session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (ScannerSessionsMap.Remove(id))
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
