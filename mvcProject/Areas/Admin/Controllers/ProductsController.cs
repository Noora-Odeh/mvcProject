using System; 
using Microsoft.AspNetCore.Mvc;
using mvcProject.Data;
using mvcProject.Models;
using mvcProject.ViewModels;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;


namespace mvcProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        public IActionResult Index()
        {
            //var products = context.Products.Join(context.Categories,
            //    p => p.CategoryId,
            //    c => c.Id,
            //    (p, c) => new
            //    {
            //        p.Name,
            //        p.Id,
            //        p.Description,
            //        p.Price,
            //        p.ImageUrl,
            //        CategoryName = c.Name
            //    });
            var products = context.Products.Include(p => p.Category).ToList();
            var productsVm = new List<ProductsViewModel>();
            foreach (var item in products)
            {
                var vm = new ProductsViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    ImageUrl = $"{Request.Scheme}://{Request.Host}/images/{item.ImageUrl}",
                    CategoryName = item.Category.Name
                };
                productsVm.Add(vm);
            }
            return View(productsVm);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = context.Categories.ToList();
            return View(new Product());
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Categories = context.Categories.ToList();
            var product = context.Products.Find(id);
            return View("Edit", product);
        }

        // Store
        public IActionResult Store(Product request, IFormFile file)
        {
            ViewBag.Categories = context.Categories.ToList();
            if (!ModelState.IsValid)
                return View("Create", request);

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("ImageUrl", "Product image is required.");
                return View("Create", request);
            }
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("ImageUrl", "Invalid image file format. Allowed: .jpg, .jpeg, .png, .gif.");
                return View("Create", request);
            }

            if (file.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("ImageUrl", "Image file size must be less than 2 MB.");
                return View("Create", request);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = System.IO.File.Create(filePath))
            {
                file.CopyTo(stream);
            }

            request.ImageUrl = fileName;
            context.Products.Add(request);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }





        // Update
        public IActionResult Update(Product request, IFormFile file)
        {
            var product = context.Products.Find(request.Id);
            if (product == null) return NotFound();

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Quantity = request.Quantity;
            product.Rate = request.Rate;
            product.CategoryId = request.CategoryId;

            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var newPath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(newPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    file.CopyTo(stream);
                }

               
                if (!string.IsNullOrWhiteSpace(product.ImageUrl))
                {
                    var oldPath = Path.Combine(uploadsFolder, product.ImageUrl);
                    SafeDelete(oldPath);
                }

                product.ImageUrl = fileName;
            }

            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int id)
        {
            var product = context.Products.Find(id);
            if (product == null) return NotFound();

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            var filePath = Path.Combine(uploadsFolder, product.ImageUrl);
            SafeDelete(filePath);

            context.Products.Remove(product);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private static void SafeDelete(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    var attrs = System.IO.File.GetAttributes(path);
                    if ((attrs & FileAttributes.ReadOnly) != 0)
                        System.IO.File.SetAttributes(path, attrs & ~FileAttributes.ReadOnly);

                    System.IO.File.Delete(path);
                }
            }
            catch {  }
        }
    }
}
