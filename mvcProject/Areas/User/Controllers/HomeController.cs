using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mvcProject.Data;
using mvcProject.Models;

namespace mvcProject.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        ApplicationDbContext context = new ApplicationDbContext();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
             var cats = context.Categories.ToList();
            ViewData ["categories"] = context.Categories.ToList();
            ViewBag.categories = context.Categories.ToList();
            return View("Index", cats);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
