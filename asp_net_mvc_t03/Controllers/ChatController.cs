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
    public  ActionResult Index(CancellationToken token)
    {
        /*var uName = HttpContext.User.Identity?.Name;
        var curUser = await _masterContext.Users
            .Where(user =>
                user.Email == uName &&
                user.Status == UserStatus.Unblock.ToString())
            .FirstOrDefaultAsync(token);
        if (curUser != null)
        {
            var topics = await _masterContext.Messages
                .Where(message =>
                    message.AuthorId == curUser.Id ||
                    message.ToUserId == curUser.Id
                )
                .OrderByDescending(message => message.CreateDate)
                .GroupBy(message => message.Head)
                .Select(g => new {name = g.Key, count = g.Count()})
                .ToListAsync(token);

            foreach (var v in topics)
            {
                Console.WriteLine(v.name);
            }*/

            //return Ok(234234324);
            return View();
        //}

        //return BadRequest();
    }
}