using Microsoft.AspNetCore.Mvc;

namespace asp_net_mvc_t03.Controllers;

public class ChatController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}