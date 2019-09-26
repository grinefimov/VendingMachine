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
        private readonly ProductContext _context;
        private readonly IHostEnvironment _hostingEnvironment;

        public AdminController(ProductContext context, IHostEnvironment env)
        {
            _context = context;
            _hostingEnvironment = env;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var model = new AdminModel(await _context.Products.ToListAsync());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProducts(AdminModel model, List<IFormFile> files)
        {
            var fileNumber = 0;
            foreach (var product in model.Products)
            {
                if (product.ImageUrl == "Changed")
                {
                    if (files[fileNumber] != null)
                    {
                        var deletePath = Path.Combine("", _hostingEnvironment.ContentRootPath + @"/wwwroot/" + product.ImageUrl);
                        if (System.IO.File.Exists(deletePath))
                        {
                            System.IO.File.Delete(deletePath);
                        }
                        var fileInfo = new FileInfo(files[fileNumber].FileName);
                        var newFilename = product.Name + fileInfo.Extension;
                        var path = Path.Combine("", _hostingEnvironment.ContentRootPath + @"\wwwroot\images\" + newFilename);
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
                if (ModelState.IsValid)
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

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(AdminModel model, IFormFile file)
        {
            if (file != null)
            {
                var fileInfo = new FileInfo(file.FileName);
                var newFilename = model.NewProduct.Name + fileInfo.Extension;
                var path = Path.Combine("", _hostingEnvironment.ContentRootPath + @"\wwwroot\images\" + newFilename);
                await using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                model.NewProduct.ImageUrl = @"/images/" + newFilename;
            }

            _context.Add(model.NewProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Delete/5
        [HttpPost]
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
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
