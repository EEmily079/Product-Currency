using Microsoft.AspNetCore.Mvc;
using Membina_Currency.Models;
using System.Drawing;
using Membina_Currency.Data;
using Microsoft.EntityFrameworkCore;
using Membina_Currency.Models.Domain;

namespace Membina_Currency.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MembinaCurrencyDbContext membinaCurrencyDbContext;
        public ProductsController(MembinaCurrencyDbContext membinaCurrencyDbContext)
        {
            this.membinaCurrencyDbContext = membinaCurrencyDbContext;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await membinaCurrencyDbContext.Products.ToListAsync();
            return View(products);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ProductList()
        {
            List<UpdateProductViewModel> productList = new List<UpdateProductViewModel>();

            var products = await membinaCurrencyDbContext.Products.ToListAsync();
            var mmkRate = await GetCurrencyRateAsync("MMK");
            var thbRate = await GetCurrencyRateAsync("THB");

            foreach (Product p in products)
            {
                var viewModel = new UpdateProductViewModel()
                {
                    Category = p.Category,
                    BrandName = p.BrandName,
                    ProductSeriesName = p.ProductSeriesName,
                    ProductName = p.ProductName,
                    Size = p.Size,
                    ProductDescription = p.ProductDescription,
                    ProductImageURL = p.ProductImageURL,
                    SgdPrice = p.SgdPrice,
                    MmkPrice = p.SgdPrice * mmkRate,
                    ThbPrice = p.SgdPrice * thbRate
                };
                productList.Add(viewModel);
            }

            return View(productList);
        }
        

        [HttpPost]
        public async Task<IActionResult> Add(AddProductViewModel addProductRequest)
        {
            var product = new Product()
            {
                Id = Guid.NewGuid(),
                Category = addProductRequest.Category,
                BrandName = addProductRequest.BrandName,
                ProductSeriesName = addProductRequest.ProductSeriesName,
                ProductName = addProductRequest.ProductName,
                Size = addProductRequest.Size,
                ProductDescription = addProductRequest.ProductDescription,
                ProductImageURL = addProductRequest.ProductImageURL,
                SgdPrice = addProductRequest.SgdPrice               
            };

            await membinaCurrencyDbContext.Products.AddAsync(product);
            await membinaCurrencyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> View(Guid id)
        {
            var mmkRate = await GetCurrencyRateAsync("MMK");
            var thbRate = await GetCurrencyRateAsync("THB");

            var product = await membinaCurrencyDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product != null)
            {

                var viewModel = new UpdateProductViewModel()
                {
                    Category = product.Category,
                    BrandName = product.BrandName,
                    ProductSeriesName = product.ProductSeriesName,
                    ProductName = product.ProductName,
                    Size = product.Size,
                    ProductDescription = product.ProductDescription,
                    ProductImageURL = product.ProductImageURL,
                    SgdPrice = product.SgdPrice,
                    MmkPrice = product.SgdPrice * mmkRate,
                    ThbPrice = product.SgdPrice*thbRate
                };
                return await Task.Run(() => View("ProductDetail", viewModel));
            }
            return RedirectToAction("ProductDetail");
        }
        private async Task<double> GetCurrencyRateAsync(string currencyName)
        {
            var currency = await membinaCurrencyDbContext.Currencies.FirstOrDefaultAsync(r => r.Name == currencyName);
            return currency?.Rate ?? 1.0; // Default to 1.0 if currency not found
        }
        [HttpPost]

        public async Task<IActionResult> View(UpdateProductViewModel model)
        {
            var product = await membinaCurrencyDbContext.Products.FindAsync(model.Id);
            if (product != null)
            {
                product.Category = model.Category;
                product.BrandName = model.BrandName;
                product.ProductSeriesName = model.ProductSeriesName;
                product.ProductName = model.ProductName;
                product.Size = model.Size;
                product.ProductDescription = model.ProductDescription;
                product.ProductImageURL = model.ProductImageURL;
                product.SgdPrice = model.SgdPrice;

                await membinaCurrencyDbContext.SaveChangesAsync();
                return RedirectToAction("ProductDetail");
            }

            return RedirectToAction("ProductDetail");
        }
    }
}