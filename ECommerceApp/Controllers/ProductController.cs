using ECommerceApp.Models;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Mvc;

public class ProductController : Controller
{
    private readonly ProductMongoService _productService;

    public ProductController(ProductMongoService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetProductsAsync();
        return View(products);
    }

    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(Product product)
    {
        if (ModelState.IsValid)
        {
            await _productService.AddProductAsync(product);
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    // Yeni eklenen action'lar
    public async Task<IActionResult> Edit(string id)
    {
        var product = await _productService.GetProductAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, Product product)
    {
        if (ModelState.IsValid)
        {
            await _productService.UpdateProductAsync(id, product);
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        await _productService.DeleteProductAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
