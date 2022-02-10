using System.ComponentModel.DataAnnotations;
using asp_net_mvc_t03.DTO.Frontend.FromBody;
using asp_net_mvc_t03.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using asp_net_mvc_t03.Models;
using Microsoft.EntityFrameworkCore;

namespace asp_net_mvc_t03.Controllers;

[Authorize]
[Route("Users"), Route("")]
public class UsersController : Controller
{
    private readonly MasterContext _masterContext;

    public UsersController(MasterContext masterContext)
    {
        _masterContext = masterContext;
    }

    [HttpGet("Index"), HttpGet("")]
    public async Task<ActionResult> IndexAsync(CancellationToken token)
    {
        var users = await _masterContext.Users.ToListAsync(token);
        return View(users);
    }

    [HttpPost("ToolButtonClick")]
    public async Task<ActionResult> ToolButtonClickAsync(CancellationToken token,
        [FromBody] [Required] ToolButtonAction data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (!Enum.TryParse(data.Action, out UserStatus status))
        {
            return BadRequest();
        }

        var usersList = await _masterContext.Users.Where(user => data.UsersId.Contains(user.Id)).ToListAsync(token);
        usersList.ForEach(user => user.Status = status.ToString());
        await _masterContext.SaveChangesAsync(token);
        return Ok();
    }
}