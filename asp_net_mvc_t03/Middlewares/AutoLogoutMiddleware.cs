using asp_net_mvc_t03.Enums;
using asp_net_mvc_t03.Models;
using Microsoft.EntityFrameworkCore;

namespace asp_net_mvc_t03.Middlewares;

public class AutoLogoutMiddleware
{
    private readonly RequestDelegate _next;

    public AutoLogoutMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, MasterContext masterContext)
    {
        if (context.User.Identity!.IsAuthenticated)
        {
            var token = context?.RequestAborted ?? CancellationToken.None;
            var uName = context!.User.Identity.Name;
            var user = await masterContext.Users
                .Where(user => user.Email == uName && user.Status == UserStatus.Unblock.ToString())
                .FirstOrDefaultAsync(token);
            if (user == null)
            {
                context.Response.Redirect("/Account/Logout");
            }
        }

        await _next(context!);
    }
}