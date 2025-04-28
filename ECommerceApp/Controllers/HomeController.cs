using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ECommerceApp.Models; // ErrorViewModel için

namespace ECommerceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Ana sayfayý direkt ürün listesine yönlendir
        public IActionResult Index()
        {
            //return View(); // Varsayýlan View'i göstermek yerine
            return RedirectToAction("Index", "Product"); // Product Controller'ýndaki Index action'ýna yönlendir
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