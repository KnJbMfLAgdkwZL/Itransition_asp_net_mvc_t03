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
        /*  var topics = await _masterContext.Messages
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

        return View();
    }

    [HttpGet("Dialog")]
    public async Task<ActionResult> Dialog([FromQuery] int id, CancellationToken token)
    {
        var uName = HttpContext.User.Identity!.Name;
        var curUser = await _masterContext.Users
            .Where(u =>
                u.Email == uName &&
                u.Status == UserStatus.Unblock.ToString())
            .FirstOrDefaultAsync(token);
        if (curUser != null)
        {
            var topics = _masterContext.Messages
                .Where(message =>
                    message.AuthorId == curUser.Id ||
                    message.ToUserId == curUser.Id)
                .Include(m => m.Author)
                .Include(m => m.ToUser)
                .AsEnumerable()
                .OrderByDescending(m => m.CreateDate)
                .GroupBy(m => m.Head)
                .Select(m => m.First())
                .ToList();

            Console.WriteLine(topics.Count);
            foreach (var v in topics)
            {
                Console.WriteLine(
                    $"{v.Id} {v.Head} {v.AuthorId} {v.ToUserId} {v.CreateDate} {v.Body} {v.Author.Email} {v.ToUser.Email}");
            }
        }

        return BadRequest();


        /*
        var message = await _masterContext.Messages
            .Where(m => m.Id == id)
            .Include(m => m.Author)
            .Include(m => m.ToUser)
            .FirstOrDefaultAsync(token);
        if (message != null)
        {
            var uName = HttpContext.User.Identity!.Name;
            var curUser = await _masterContext.Users
                .Where(u =>
                    u.Email == uName &&
                    u.Status == UserStatus.Unblock.ToString())
                .FirstOrDefaultAsync(token);
            if (curUser != null)
            {
                if (message.AuthorId == curUser.Id || message.ToUserId == curUser.Id)
                {
                    var dialogUsersId = new List<int>()
                    {
                        message.AuthorId,
                        message.ToUserId
                    };

                    var messages = await _masterContext.Messages
                        .Where(m =>
                            dialogUsersId.Contains(m.AuthorId) &&
                            dialogUsersId.Contains(m.ToUserId) &&
                            m.Head == message.Head
                        )
                        .Include(m => m.Author)
                        .Include(m => m.ToUser)
                        .OrderBy(m => m.CreateDate).ToListAsync(token);

                    return View(messages);
                }
            }
        }

        return BadRequest();*/
    }
}