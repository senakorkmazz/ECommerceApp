using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using ECommerceApp.Data;
using ECommerceApp.Models;
using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Bu kullanıcı adı zaten kullanılıyor.");
                    return View(user);
                }

                user.PasswordHash = HashPassword(user.PasswordHash);

                _context.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Login));
            }
            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

                if (user != null && user.PasswordHash == HashPassword(model.Password))
                {
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetInt32("UserId", user.Id);

                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
                }
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); 
            return RedirectToAction("Index", "Home");
        }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}