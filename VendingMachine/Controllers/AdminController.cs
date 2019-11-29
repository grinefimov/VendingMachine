using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using VendingMachine.Data;
using VendingMachine.Models;

namespace VendingMachine.Controllers
{
    public class AdminController : Controller
    {
        private readonly string _secretKey = "SecretKey";
        private readonly VendingMachineContext _context;
        private readonly IHostEnvironment _hostingEnvironment;

        public AdminController(VendingMachineContext context, IHostEnvironment env)
        {
            _context = context;
            _hostingEnvironment = env;
        }

        public async Task<IActionResult> Panel(string key)
        {
            if (key == _secretKey)
            {
                var model = new AdminPanelModel()
                {
                    Products = await _context.Products.ToListAsync(),
                    Cashes = await _context.Cashes.ToListAsync()
                };

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveData(AdminPanelModel model, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (model.Products != null)
                {
                    var fileNumber = 0;
                    foreach (var product in model.Products)
                    {
                        if (new string(product.ImageUrl.ToCharArray()[..10]) == "(Changed) ")
                        {
                            if (files[fileNumber] != null)
                            {
                                var deletePath = Path.Combine("",
                                    _hostingEnvironment.ContentRootPath + @"/wwwroot/" + new string(product.ImageUrl.ToCharArray()[10..]));
                                if (System.IO.File.Exists(deletePath))
                                {
                                    System.IO.File.Delete(deletePath);
                                }

                                var newFilename = CreateFileName(product.Name, files[fileNumber].FileName);
                                var path = Path.Combine("",
                                    _hostingEnvironment.ContentRootPath + @"\wwwroot\images\" + newFilename);
                                await using (var stream = new FileStream(path, FileMode.Create))
                                {
                                    await files[fileNumber].CopyToAsync(stream);
                                }

                                product.ImageUrl = @"/images/" + newFilename;
                                fileNumber++;
                            }
                        }
                    }

                    foreach (var product in model.Products)
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
                }

                if (model.Cashes != null)
                {
                    foreach (var cash in model.Cashes)
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
                }
            }

            return RedirectToAction(nameof(Panel), new {key = _secretKey});
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(AdminPanelModel model, IFormFile file)
        {
            if (file != null)
            {
                var newFilename = CreateFileName(model.NewProduct.Name, file.FileName);
                var path = Path.Combine("", _hostingEnvironment.ContentRootPath + @"\wwwroot\images\" + newFilename);
                await using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                model.NewProduct.ImageUrl = @"/images/" + newFilename;
            }

            _context.Add(model.NewProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Panel), new {key = _secretKey});
        }

        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            var path = Path.Combine("", _hostingEnvironment.ContentRootPath + @"/wwwroot/" + product.ImageUrl);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            return RedirectToAction(nameof(Panel), new {key = _secretKey});
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        private bool CashExists(int id)
        {
            return _context.Cashes.Any(e => e.Id == id);
        }

        private static string CreateFileName(string name, string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            var now = DateTime.Now;
            var time = " " + now.Day + "-" + now.Month + "-" + now.Year + " " + now.Hour + "-" + now.Minute;
            return name + time + fileInfo.Extension;
        }
    }
}