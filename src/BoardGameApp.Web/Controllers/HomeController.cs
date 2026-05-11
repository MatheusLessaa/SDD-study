using Microsoft.AspNetCore.Mvc;

namespace BoardGameApp.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
