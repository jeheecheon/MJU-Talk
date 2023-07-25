using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MJU_Talk.Controllers;

public class HomeController : Controller
{
    public HomeController() { }

    public IActionResult Index()
    {
        return View("Home", "Home Page");
    }
}