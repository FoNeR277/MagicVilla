using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_Web.Controllers;

public class VillaController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}