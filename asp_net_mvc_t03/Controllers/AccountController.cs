using asp_net_mvc_t03.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Application.DTO.Frontend.Forms;
using asp_net_mvc_t03.Enums;
using Microsoft.EntityFrameworkCore;

namespace asp_net_mvc_t03.Controllers;

[Route("Account")]
public class AccountController : Controller
{
    private readonly MasterContext _masterContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AccountController(MasterContext masterContext, IPasswordHasher<User> passwordHasher)
    {
        _masterContext = masterContext;
        _passwordHasher = passwordHasher;
    }

    [AllowAnonymous]
    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginAsync(CancellationToken token, [FromForm] LoginForm loginForm)
    {
        if (ModelState.IsValid)
        {
            var user = await _masterContext.Users.Where(user =>
                    user.Email == loginForm.Email && user.Status == UserStatus.Unblock.ToString())
                .FirstOrDefaultAsync(token);
            if (user != null)
            {
                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user,
                    user.Password,
                    loginForm.Password);

                if (passwordVerificationResult == PasswordVerificationResult.Success)
                {
                    await AuthenticateAsync(user.Email);

                    user.LastLoginDate = DateTime.Now;
                    var u = _masterContext.Users.Update(user);
                    await _masterContext.SaveChangesAsync(token);

                    return RedirectToAction("Index", "Users");
                }
            }

            ModelState.AddModelError("", "Incorrect login or password");
        }

        return View(loginForm);
    }

    [AllowAnonymous]
    [HttpGet("Register")]
    public IActionResult Register()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterAsync(CancellationToken token, [FromForm] RegisterForm registerForm)
    {
        if (ModelState.IsValid)
        {
            var user = await _masterContext.Users.Where(user => user.Email == registerForm.Email)
                .FirstOrDefaultAsync(token);
            if (user == null)
            {
                var userNew = new User
                {
                    Email = registerForm.Email,
                    Password = registerForm.Password,
                    Name = registerForm.Name,
                    RegistrationDate = DateTime.Now,
                    Status = UserStatus.Unblock.ToString()
                };
                userNew.Password = _passwordHasher.HashPassword(userNew, registerForm.Password);

                await _masterContext.Users.AddAsync(userNew, token);
                await _masterContext.SaveChangesAsync(token);

                await AuthenticateAsync(userNew.Email);

                userNew.LastLoginDate = DateTime.Now;
                var u = _masterContext.Users.Update(userNew);
                await _masterContext.SaveChangesAsync(token);

                return RedirectToAction("Index", "Users");
            }

            ModelState.AddModelError("", "Incorrect login or password");
        }

        return View(registerForm);
    }

    [Authorize]
    [HttpGet("Logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    private async Task AuthenticateAsync(string userName)
    {
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, userName),
        };
        var id = new ClaimsIdentity(
            claims, "ApplicationCookie",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
    }
}