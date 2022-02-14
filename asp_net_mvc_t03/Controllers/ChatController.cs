using asp_net_mvc_t03.Enums;
using asp_net_mvc_t03.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace asp_net_mvc_t03.Controllers;

[Route("Chat")]
public class ChatController : Controller
{
    private readonly MasterContext _masterContext;

    public ChatController(MasterContext masterContext)
    {
        _masterContext = masterContext;
    }

    [HttpGet("Index")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("GetAddressees")]
    public async Task<ActionResult> GetAddresseesAsync(CancellationToken token, [FromQuery] string search)
    {
        var listString = await _masterContext.Users
            .Where(user =>
                EF.Functions.Like(user.Email, $"%{search}%") &&
                user.Status == UserStatus.Unblock.ToString())
            .OrderBy(user => user.Email)
            .Take(5)
            .Select(user => user.Email)
            .ToListAsync(token);
        return Ok(listString);
    }
}