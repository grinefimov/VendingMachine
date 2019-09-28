using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VendingMachine.Data;
using VendingMachine.Models;

namespace VendingMachine.Controllers
{
    public class ClientController : Controller
    {
        private readonly ILogger<ClientController> _logger;
        private readonly ProductContext _productContext;
        private readonly CashContext _cashContext;

        public ClientController(ILogger<ClientController> logger, ProductContext productContext,
            CashContext cashContext)
        {
            _logger = logger;
            _productContext = productContext;
            _cashContext = cashContext;
        }

        public async Task<IActionResult> Index()
        {
            var model = new VendingMachineModel()
            {
                Products = await _productContext.Products.ToListAsync(),
                Cashes = await _cashContext.Cashes.ToListAsync()
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        [HttpPost]
        public async Task<IActionResult> Buy(List<Product> products, List<int> newCashIds, float change)
        {
            var cashes = await _cashContext.Cashes.OrderByDescending(c => c.FaceValue).ToListAsync();

            foreach (var cash in cashes)
            {
                foreach (var cashId in newCashIds)
                {
                    if (cash.Id == cashId)
                    {
                        cash.Quantity++;
                    }
                }
            }

            var changeCashes = new List<float>();
            var tempChange = change;
            foreach (var cash in cashes)
            {
                while (tempChange >= cash.FaceValue && cash.Quantity > 0)
                {
                    tempChange -= cash.FaceValue;
                    cash.Quantity--;
                    changeCashes.Add(cash.FaceValue);
                }
            }

            bool noCashLeft = tempChange > 0 && cashes[cashes.Count - 1].Quantity == 0;

            foreach (var product in products)
            {
                try
                {
                    _productContext.Update(product);
                    await _productContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            foreach (var cash in cashes)
            {
                try
                {
                    _cashContext.Update(cash);
                    await _cashContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CashExists(cash.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return Json(new {change = change, changeCashes = changeCashes, noCashLeft = noCashLeft});
        }

        private bool ProductExists(int id)
        {
            return _productContext.Products.Any(e => e.Id == id);
        }

        private bool CashExists(int id)
        {
            return _cashContext.Cashes.Any(e => e.Id == id);
        }
    }
}