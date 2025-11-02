using Microsoft.AspNetCore.Mvc;
using mvcProject.Data;
using mvcProject.Models;
using System.Threading.Tasks;

namespace mvcProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var products = context.Products.ToList();
            return View(products);
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
            return View("Create", product);
        }

        public  IActionResult Store(Product request,IFormFile file)
        {
            if (file!=null && file.Length>0)
            {
                var fileName = Guid.NewGuid().ToString();
                fileName += Path.GetExtension(file.FileName);
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images", fileName);

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                   file.CopyToAsync(stream);
                }

                request.ImageUrl = fileName;
                context.Products.Add(request);
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = context.Categories.ToList();
            return View("Create",request);
        }

        public IActionResult Update(Product request, IFormFile file)
        {
            var product = context.Products.Find(request.Id);

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.CategoryId = request.CategoryId;

            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString();
                fileName += Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images",fileName);

         
                 using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
                // Delete old image
        
                product.ImageUrl = fileName;
            }
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int id) {
            var product = context.Products.Find(id);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images",product.ImageUrl);
            System.IO.File.Delete(filePath);
            context.Products.Remove(product);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
