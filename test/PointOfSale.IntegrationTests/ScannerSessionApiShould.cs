using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using PointOfSale.Api;
using PointOfSale.Api.Models;
using Xunit;

namespace PointOfSale.IntegrationTests
{
    public class ScannerSessionApiShould : IClassFixture<WebApplicationFactory<Startup>>
    {
        const string ScannerApi = "/api/scanner-session";
        private static readonly Random _random = new Random();

        private static readonly HttpContent EmptyContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        
        private readonly WebApplicationFactory<Startup> _factory;

        public ScannerSessionApiShould(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateStartScannerSession()
        {
            var client = _factory.CreateClient();

            var createSessionResponse = await client.PostAsync(ScannerApi, EmptyContent);
            createSessionResponse.EnsureSuccessStatusCode();
            var sessionUri = createSessionResponse.Headers.Location.LocalPath;

            Assert.Equal(HttpStatusCode.Created, createSessionResponse.StatusCode);

            var scannerSessionResponse = await client.GetStringAsync($"{sessionUri}");
            var scannerSession = JsonConvert.DeserializeObject<ScannerSessionResponseModel>(scannerSessionResponse);

            Assert.Empty(scannerSession.ScannedProducts);
        }

        [Fact]
        public async Task AddProductFromProductCatalog()
        {
            var client = _factory.CreateClient();

            var createSessionResponse = await client.PostAsync(ScannerApi, EmptyContent);
            var sessionUri = createSessionResponse.Headers.Location.LocalPath;

            var productCode = "A";
            var scanProductResponse = await client.PostAsync($"{sessionUri}/products/{productCode}", EmptyContent);
            scanProductResponse.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, scanProductResponse.StatusCode);

            var scannerSessionResponse = await client.GetStringAsync($"{sessionUri}");
            var scannerSession = JsonConvert.DeserializeObject<ScannerSessionResponseModel>(scannerSessionResponse);

            Assert.Contains(scannerSession.ScannedProducts, x => x.Code == productCode && x.ProductsCount == 1);
        }

        [Fact]
        public async Task AddProductInAmountMoreThanOneFromProductCatalog()
        {
            var client = _factory.CreateClient();

            var createSessionResponse = await client.PostAsync(ScannerApi, EmptyContent);
            var sessionUri = createSessionResponse.Headers.Location.LocalPath;

            var productCode = "A";
            var productAmount = 10;
            for (int i = 0; i < productAmount; i++)
            {
                var scanProductResponse = await client.PostAsync($"{sessionUri}/products/{productCode}", EmptyContent);
                scanProductResponse.EnsureSuccessStatusCode();

                Assert.Equal(HttpStatusCode.OK, scanProductResponse.StatusCode);
            }

            var scannerSessionResponse = await client.GetStringAsync($"{sessionUri}");
            var scannerSession = JsonConvert.DeserializeObject<ScannerSessionResponseModel>(scannerSessionResponse);

            Assert.Contains(scannerSession.ScannedProducts, x => x.Code == productCode && x.ProductsCount == productAmount);
        }

        [Fact]
        public async Task ReturnNotFoundOnAddingUnknownProduct()
        {
            var client = _factory.CreateClient();

            var createSessionResponse = await client.PostAsync(ScannerApi, EmptyContent);
            var sessionUri = createSessionResponse.Headers.Location.LocalPath;

            var productCode = "X";
            var scanProductResponse = await client.PostAsync($"{sessionUri}/products/{productCode}", EmptyContent);
            Assert.Throws<HttpRequestException>(() => scanProductResponse.EnsureSuccessStatusCode());

            Assert.Equal(HttpStatusCode.NotFound, scanProductResponse.StatusCode);
        }

        [Fact]
        public async Task ReturnTotalPriceWithoutDiscountsForSingleProductWithoutSpecialOffer()
        {
            var client = _factory.CreateClient();

            var productWithoutSpecialOffer = (await GetProducts()).First(x => x.SpecialOffer == null);
            var productCode = productWithoutSpecialOffer.Code;
            var productPrice = productWithoutSpecialOffer.Price;

            var createSessionResponse = await client.PostAsync(ScannerApi, EmptyContent);
            var sessionUri = createSessionResponse.Headers.Location.LocalPath;

            var scanProductResponse = await client.PostAsync($"{sessionUri}/products/{productCode}", EmptyContent);
            scanProductResponse.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, scanProductResponse.StatusCode);
            var totalPriceResponse = double.Parse(await client.GetStringAsync($"{sessionUri}/total"));

            Assert.Equal(1 * productPrice, totalPriceResponse);
        }

        [Fact]
        public async Task ReturnTotalPriceWithoutDiscountForVolumeOfProductLessThanInSpecialOffer()
        {
            var client = _factory.CreateClient();

            var productWithSpecialOffer = (await GetProducts()).First(x => x.SpecialOffer != null);
            var productCode = productWithSpecialOffer.Code;
            var productPrice = productWithSpecialOffer.Price;

            var createSessionResponse = await client.PostAsync(ScannerApi, EmptyContent);
            var sessionUri = createSessionResponse.Headers.Location.LocalPath;

            var productsCount = productWithSpecialOffer.SpecialOffer.Amount - 1;
            for (int i = 0; i < productsCount; i++)
            {
                var scanProductResponse = await client.PostAsync($"{sessionUri}/products/{productCode}", EmptyContent);
                scanProductResponse.EnsureSuccessStatusCode();
            }

            var totalPriceResponse = double.Parse(await client.GetStringAsync($"{sessionUri}/total"));

            Assert.Equal(productsCount * productPrice, totalPriceResponse);
        }

        [Fact]
        public async Task ReturnTotalPriceWithDiscountForVolumeOfProductEqualToSpecialOffer()
        {
            var client = _factory.CreateClient();

            var productWithSpecialOffer = (await GetProducts()).First(x => x.SpecialOffer != null);
            var productCode = productWithSpecialOffer.Code;

            var createSessionResponse = await client.PostAsync(ScannerApi, EmptyContent);
            var sessionUri = createSessionResponse.Headers.Location.LocalPath;

            var productsCount = productWithSpecialOffer.SpecialOffer.Amount;
            for (int i = 0; i < productsCount; i++)
            {
                var scanProductResponse = await client.PostAsync($"{sessionUri}/products/{productCode}", EmptyContent);
                scanProductResponse.EnsureSuccessStatusCode();
            }

            var totalPriceResponse = double.Parse(await client.GetStringAsync($"{sessionUri}/total"));

            Assert.Equal(productWithSpecialOffer.SpecialOffer.TotalPrice, totalPriceResponse);
        }

        [Fact]
        public async Task CalculateTotalPriceCorectlyForMoreThanOneProduct()
        {
            var client = _factory.CreateClient();

            var createSessionResponse = await client.PostAsync(ScannerApi, EmptyContent);
            var sessionUri = createSessionResponse.Headers.Location.LocalPath;
            foreach (var product in await GetProducts())
            {
                var productsCount = _random.Next(1, 10);
                for (int i = 0; i < productsCount; i++)
                {
                    var scanProductResponse =
                        await client.PostAsync($"{sessionUri}/products/{product.Code}", EmptyContent);
                    scanProductResponse.EnsureSuccessStatusCode();
                }
            }

            var scannerSessionResponse = await client.GetStringAsync($"{sessionUri}");
            var scannerSession = JsonConvert.DeserializeObject<ScannerSessionResponseModel>(scannerSessionResponse);

            double expectedTotalPrice = 0;
            foreach (var scannedProduct in scannerSession.ScannedProducts)
            {
                var priceWithoutDiscount = scannedProduct.Price * scannedProduct.ProductsCount;
                if (scannedProduct.SpecialOffer == null)
                {
                    expectedTotalPrice += priceWithoutDiscount;

                }
                else
                {
                    var specialOffersCount = scannedProduct.ProductsCount / scannedProduct.SpecialOffer.Amount;
                    var perOfferDiscount = scannedProduct.SpecialOffer.Amount * scannedProduct.Price -
                                           scannedProduct.SpecialOffer.TotalPrice;
                    expectedTotalPrice += priceWithoutDiscount - specialOffersCount * perOfferDiscount;
                }
            }

            var totalPriceResponse = double.Parse(await client.GetStringAsync($"{sessionUri}/total"));

            Assert.Equal(expectedTotalPrice, totalPriceResponse);
        }

        private async Task<ProductResponseModel[]> GetProducts()
        {
            var client = _factory.CreateClient();

            var response = await client.GetStringAsync("api/products");
            return JsonConvert.DeserializeObject<ProductResponseModel[]>(response);
        }
    }
}
