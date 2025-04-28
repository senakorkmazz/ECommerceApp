using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ECommerceApp.Models; // ErrorViewModel i�in

namespace ECommerceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Ana sayfay� direkt �r�n listesine y�nlendir
        public IActionResult Index()
        {
            //return View(); // Varsay�lan View'i g�stermek yerine
            return RedirectToAction("Index", "Product"); // Product Controller'�ndaki Index action'�na y�nlendir
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