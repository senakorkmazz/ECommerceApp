using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Include Eager Loading için
using System.Linq; // FirstOrDefaultAsync için
using System.Threading.Tasks; // async/await için
using System.Security.Cryptography; // Basit Hashleme için
using System.Text; // Encoding için
using Microsoft.AspNetCore.Http;
using ECommerceApp.Data;
using ECommerceApp.Models;
using System.ComponentModel.DataAnnotations; // Session için

namespace ECommerceApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Basit ve GÜVENSİZ Hashleme Fonksiyonu - SADECE DEMO AMAÇLIDIR!
        // GERÇEK UYGULAMALARDA ASLA KULLANMAYIN! BCrypt.Net gibi kütüphaneler kullanın.
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        // Kayıt Sayfası (GET)
        public IActionResult Register()
        {
            return View();
        }

        // Kayıt İşlemi (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcı adı zaten var mı?
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Bu kullanıcı adı zaten kullanılıyor.");
                    return View(user);
                }

                // Şifreyi hash'le (Demo amaçlı basit hash)
                user.PasswordHash = HashPassword(user.PasswordHash); // Modeldeki alan adını PasswordHash varsaydık

                _context.Add(user);
                await _context.SaveChangesAsync();

                // Kayıt sonrası login sayfasına yönlendir
                return RedirectToAction(nameof(Login));
            }
            return View(user);
        }

        // Giriş Sayfası (GET)
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model) // Ayrı bir ViewModel kullanmak daha iyi
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

                if (user != null && user.PasswordHash == HashPassword(model.Password))
                {
                    // Giriş başarılı - Session'a kullanıcı adını kaydet
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetInt32("UserId", user.Id); // Kullanıcı ID'sini de saklayabiliriz

                    // Check if there's a return URL in TempData
                    if (TempData["ReturnUrl"] != null)
                    {
                        string returnUrl = TempData["ReturnUrl"].ToString();
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home"); // Ana sayfaya yönlendir
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
                }
            }
            return View(model);
        }

        // Çıkış İşlemi (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Session'ı temizle
            return RedirectToAction("Index", "Home"); // Ana sayfaya yönlendir
        }
    }

    // Basit Login ViewModel'ı (Models klasörüne veya ayrı bir ViewModels klasörüne koyabilirsiniz)
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}