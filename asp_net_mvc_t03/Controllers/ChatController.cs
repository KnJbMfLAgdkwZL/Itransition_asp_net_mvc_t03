using System.ComponentModel.DataAnnotations;
using asp_net_mvc_t03.Enums;
using asp_net_mvc_t03.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;

namespace asp_net_mvc_t03.Controllers;

[Authorize]
[Route("Chat")]
public class ChatController : Controller
{
    private readonly MasterContext _masterContext;

    public ChatController(MasterContext masterContext)
    {
        _masterContext = masterContext;
    }

    [HttpGet("Index")]
    public ActionResult Index(CancellationToken token)
    {
        return View();
    }

    [HttpGet("Dialog")]
    public async Task<ActionResult> Dialog([FromQuery] string? uid, CancellationToken token)
    {
        if (uid == null)
        {
            return BadRequest();
        }

        if (HttpContext.User.Identity!.IsAuthenticated == false)
        {
            return Unauthorized();
        }

        var uName = HttpContext.User.Identity!.Name;
        var curUser = await _masterContext.Users
            .Where(u =>
                u.Email == uName &&
                u.Status == UserStatus.Unblock.ToString())
            .FirstOrDefaultAsync(token);
        if (curUser == null)
        {
            return Unauthorized();
        }

        var message = await _masterContext.Messages
            .Where(m =>
                (m.AuthorId == curUser.Id || m.ToUserId == curUser.Id) &&
                m.Uid == uid
            )
            .Include(m => m.Author)
            .Include(m => m.ToUser)
            .OrderBy(m => m.CreateDate)
            .FirstOrDefaultAsync(token);
        if (message == null)
        {
            return BadRequest();
        }

        return View(message);
    }
}