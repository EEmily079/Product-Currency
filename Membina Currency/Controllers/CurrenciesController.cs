using Membina_Currency.Data;
using Membina_Currency.Models.Domain;
using Membina_Currency.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Membina_Currency.Controllers
{
    public class CurrenciesController : Controller
    {
        private readonly MembinaCurrencyDbContext membinaCurrencyDbContext;
        public CurrenciesController(MembinaCurrencyDbContext membinaCurrencyDbContext)
        {
            this.membinaCurrencyDbContext = membinaCurrencyDbContext;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currencies = await membinaCurrencyDbContext.Currencies.ToListAsync();
            return View(currencies);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCurrencyViewModel addCurrRequest)
        {
            var currency = new Currency()
            {
                Id = Guid.NewGuid(),
                Name = addCurrRequest.Name,
                Rate = addCurrRequest.Rate               
            };

            await membinaCurrencyDbContext.Currencies.AddAsync(currency);
            await membinaCurrencyDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> View(Guid id)
        {
            var currency = await membinaCurrencyDbContext.Currencies.FirstOrDefaultAsync(x => x.Id == id);
            if (currency != null)
            {
                var viewModel = new UpdateCurrencyViewModel()
                {
                    Id = currency.Id,
                    Name = currency.Name,
                    Rate = currency.Rate               
                };
                return await Task.Run(() => View("View", viewModel));
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> View(UpdateCurrencyViewModel model)
        {
            var currency = await membinaCurrencyDbContext.Currencies.FindAsync(model.Id);
            if (currency != null)
            {
                currency.Name = model.Name;
                currency.Rate = model.Rate;
               
                await membinaCurrencyDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }

}

