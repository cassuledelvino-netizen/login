using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using login.Models;

namespace login.Controllers; //xsmtpsib-1a98310248243137d462b87a6ca6914b06ecce124abaecb037bb8df1740adb66-9mw4GRfCf0QTLvRW

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
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
