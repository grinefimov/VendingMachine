using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingMachine.Data;
using VendingMachine.Models;

namespace VendingMachine.Controllers
{
    public class ClientController : Controller
    {
        private readonly VendingMachineContext _context;

        public ClientController(VendingMachineContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new VendingMachineModel()
            {
                Products = await _context.Products.ToListAsync(),
                Cashes = await _context.Cashes.ToListAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> GetData()
        {
            var products = await _context.Products.ToListAsync();
            var cashes = await _context.Cashes.ToListAsync();

            return Json(new { products, cashes });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        [HttpPost]
        public async Task<IActionResult> Buy(List<Product> checkout, List<int> newCashIds, float change)
        {
            var products = await _context.Products.ToListAsync();
            var cashes = await _context.Cashes.OrderByDescending(c => c.FaceValue).ToListAsync();

            foreach (var product in products)
            {
                if (product.Quantity > 0)
                {
                    foreach (var buy in checkout)
                    {
                        if (product.Id == buy.Id)
                        {
                            product.Quantity--;
                        }
                    }
                }
            }

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
                    _context.Update(product);
                    await _context.SaveChangesAsync();
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
                    _context.Update(cash);
                    await _context.SaveChangesAsync();
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

            return Json(new {change, changeCashes, noCashLeft});
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        private bool CashExists(int id)
        {
            return _context.Cashes.Any(e => e.Id == id);
        }
    }
}