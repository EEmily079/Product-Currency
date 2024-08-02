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
                    Id=p.Id,
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
                productList = productList.OrderBy(p => p.BrandName).ToList();

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

            // Check if a product with the same details already exists
            var existingProduct = membinaCurrencyDbContext.Products
                .FirstOrDefault(p => p.Category == product.Category &&
                                     p.BrandName == product.BrandName &&
                                     p.ProductSeriesName == product.ProductSeriesName &&
                                     p.ProductName == product.ProductName &&
                                     p.Size == product.Size);

            if (existingProduct != null)
            {
                // Use TempData to pass the error message
                TempData["ErrorMessage"] = "Product already exists";
                return RedirectToAction("Add"); // Redirect to the same Add view to show the alert
            }

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
                    Id= product.Id,
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
                return View("ProductDetail", viewModel);
            }
            return RedirectToAction("Index");
        }
        private async Task<double> GetCurrencyRateAsync(string currencyName)
        {
            var currency = await membinaCurrencyDbContext.Currencies.FirstOrDefaultAsync(r => r.Name == currencyName);
            return currency?.Rate ?? 1.0; // Default to 1.0 if currency not found
        }



        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var mmkRate = await GetCurrencyRateAsync("MMK");
            var thbRate = await GetCurrencyRateAsync("THB");

            var product = await membinaCurrencyDbContext.Products.FindAsync(id);
            if (product != null)
            {

                var updateProductViewModel = new UpdateProductViewModel()
                {
                    Id = id,
                    Category = product.Category,
                    BrandName = product.BrandName,
                    ProductSeriesName = product.ProductSeriesName,
                    ProductName = product.ProductName,
                    Size = product.Size,
                    ProductDescription = product.ProductDescription,
                    ProductImageURL = product.ProductImageURL,
                    SgdPrice = product.SgdPrice,
                };
                return View(updateProductViewModel);
            }
            return NotFound();
        }

        [HttpPost]
        [Route("Products/Update/{id:guid}")]
        public async Task<IActionResult> Update(UpdateProductViewModel updateProductRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(updateProductRequest);
            }

            var product = await membinaCurrencyDbContext.Products.FindAsync(updateProductRequest.Id);
            if (product == null)
            {
                return NotFound();
            }

            product.Category = updateProductRequest.Category;
            product.BrandName = updateProductRequest.BrandName;
            product.ProductSeriesName = updateProductRequest.ProductSeriesName;
            product.ProductName = updateProductRequest.ProductName;
            product.Size = updateProductRequest.Size;
            product.ProductDescription = updateProductRequest.ProductDescription;
            product.ProductImageURL = updateProductRequest.ProductImageURL;
            product.SgdPrice = updateProductRequest.SgdPrice;
             
            await membinaCurrencyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var productToDelete = await membinaCurrencyDbContext.Products.FindAsync(id);
            if (productToDelete != null)
            {
               
                membinaCurrencyDbContext.Products.Remove(productToDelete);
                membinaCurrencyDbContext.SaveChanges();

                return RedirectToAction("ProductList");
            }
            return RedirectToAction("ProductList");
        }








    }
}