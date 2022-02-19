using System.Security.Claims;
using asp_net_mvc_t03.Enums;
using asp_net_mvc_t03.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace asp_net_mvc_t03.AuthorizationHandler;

public class CustomAuthorizationHandler : IAuthorizationHandler
{
    private readonly MasterContext _masterContext;

    public CustomAuthorizationHandler(MasterContext masterContext)
    {
        _masterContext = masterContext;
    }

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var nameClaim = context.User.FindFirst(c => c.Type == ClaimTypes.Name);
        if (nameClaim is not null)
        {
            var user = await _masterContext.Users
                .Where(user =>
                    user.Email == nameClaim.Value &&
                    user.Status == UserStatus.Unblock.ToString())
                .FirstOrDefaultAsync(CancellationToken.None);
            if (user != null)
            {
                context.Succeed(new AssertionRequirement(handlerContext => handlerContext.HasSucceeded));
                return;
            }
        }

        context.Fail();
    }
}