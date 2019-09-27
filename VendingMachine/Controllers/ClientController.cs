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

        public ClientController(ILogger<ClientController> logger, ProductContext productContext, CashContext cashContext)
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
