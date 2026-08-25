using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VasilyTest.Web.Data;
using VasilyTest.Web.Models;

namespace VasilyTest.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _db.Products.ToListAsync();
        return View(products);
    }

    [HttpPost]
    public async Task<IActionResult> Save(int id, string name, decimal price)
    {
        Product product = new Product();

        if (id > 0)
        {
            product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            product.Name = name;
            product.Price = price;
        }
        else
        {
            product = new Product();
            product.Name = name;
            product.Price = price;
            product.CreatedAt = DateTime.Now;
            _db.Products.Add(product);
        }

        await _db.SaveChangesAsync();

        return Json(new
        {
            id = product.Id,
            name = product.Name,
            price = product.Price.ToString("0.00"),
            createdAt = product.CreatedAt.ToString("yyyy-MM-dd HH:mm")
        });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        return Json(new { ok = true });
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}