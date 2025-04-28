using ECommerceApp.Models;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

public class ProductController : Controller
{
    private readonly ProductService _productService;

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    private bool IsUserLoggedIn()
    {
        return !string.IsNullOrEmpty(HttpContext.Session.GetString("Username"));
    }

    private IActionResult RedirectToLogin(string returnUrl = null)
    {
        
        return RedirectToAction("Login", "Account");
    }


    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetProductsAsync();
        return View(products);
    }

    public IActionResult Add()
    {
        
        if (!IsUserLoggedIn())
        {
            return RedirectToLogin("/Product/Add");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Product product)
    {
        
        if (!IsUserLoggedIn())
        {
            return RedirectToLogin();
        }

        if (ModelState.IsValid)
        {
            await _productService.AddProductAsync(product);
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    public async Task<IActionResult> Edit(string id)
    {
        if (!IsUserLoggedIn())
        {
            return RedirectToLogin($"/Product/Edit/{id}");
        }

        var product = await _productService.GetProductAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Product product)
    {
        if (!IsUserLoggedIn())
        {
            return RedirectToLogin();
        }

        
        ModelState.Remove("Id");

        if (id != product.MongoId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                
                var existingProduct = await _productService.GetProductAsync(id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                product.Id = existingProduct.Id;

                await _productService.UpdateProductAsync(id, product);
                TempData["SuccessMessage"] = "Ürün başarıyla güncellendi!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Güncelleme hatası: {ex.Message}";
            }
        }

        return View(product);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        
        if (!IsUserLoggedIn())
        {
            return RedirectToLogin();
        }

        await _productService.DeleteProductAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
