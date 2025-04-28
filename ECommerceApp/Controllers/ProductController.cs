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

    // Helper method to check if user is logged in
    private bool IsUserLoggedIn()
    {
        return !string.IsNullOrEmpty(HttpContext.Session.GetString("Username"));
    }

    // Helper method to redirect to login page
    private IActionResult RedirectToLogin(string returnUrl = null)
    {
        if (!string.IsNullOrEmpty(returnUrl))
        {
            TempData["ReturnUrl"] = returnUrl;
        }
        return RedirectToAction("Login", "Account");
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetProductsAsync();
        return View(products);
    }

    public IActionResult Add()
    {
        // Check if user is logged in
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
        // Check if user is logged in
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
        // Check if user is logged in
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
        // Check if user is logged in
        if (!IsUserLoggedIn())
        {
            return RedirectToLogin();
        }

        if (ModelState.IsValid)
        {
            await _productService.UpdateProductAsync(id, product);
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        // Check if user is logged in
        if (!IsUserLoggedIn())
        {
            return RedirectToLogin();
        }

        await _productService.DeleteProductAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
