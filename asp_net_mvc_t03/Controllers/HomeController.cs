using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using asp_net_mvc_t03.Models;
using Microsoft.AspNetCore.Authorization;

namespace asp_net_mvc_t03.Controllers;

[AllowAnonymous]
[Route("Home")]
public class HomeController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}