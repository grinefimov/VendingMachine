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
        private readonly ProductContext _productContext;
        private readonly CashContext _cashContext;
        private readonly IHostEnvironment _hostingEnvironment;

        public AdminController(ProductContext productContext, CashContext cashContext, IHostEnvironment env)
        {
            _productContext = productContext;
            _cashContext = cashContext;
            _hostingEnvironment = env;
        }

        public async Task<IActionResult> Panel(string key)
        {
            if (key == _secretKey)
            {
                var model = new AdminPanelModel()
                {
                    Products = await _productContext.Products.ToListAsync(),
                    Cashes = await _cashContext.Cashes.ToListAsync()
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
                        if (product.ImageUrl == "Changed")
                        {
                            if (files[fileNumber] != null)
                            {
                                var deletePath = Path.Combine("",
                                    _hostingEnvironment.ContentRootPath + @"/wwwroot/" + product.ImageUrl);
                                if (System.IO.File.Exists(deletePath))
                                {
                                    System.IO.File.Delete(deletePath);
                                }

                                var fileInfo = new FileInfo(files[fileNumber].FileName);
                                var newFilename = product.Name + fileInfo.Extension;
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
                }

                if (model.Cashes != null)
                {
                    foreach (var cash in model.Cashes)
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
                var fileInfo = new FileInfo(file.FileName);
                var now = DateTime.Now;
                string time = " " + now.Day + "-" + now.Month + "-" + now.Year + " " + now.Hour + "-" + now.Minute;
                var newFilename = model.NewProduct.Name + time + fileInfo.Extension;
                var path = Path.Combine("", _hostingEnvironment.ContentRootPath + @"\wwwroot\images\" + newFilename);
                await using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                model.NewProduct.ImageUrl = @"/images/" + newFilename;
            }

            _productContext.Add(model.NewProduct);
            await _productContext.SaveChangesAsync();

            return RedirectToAction(nameof(Panel), new {key = _secretKey});
        }

        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productContext.Products.FindAsync(id);
            _productContext.Products.Remove(product);
            await _productContext.SaveChangesAsync();
            var path = Path.Combine("", _hostingEnvironment.ContentRootPath + @"/wwwroot/" + product.ImageUrl);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            return RedirectToAction(nameof(Panel), new {key = _secretKey});
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