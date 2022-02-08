using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Threading;
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
        Console.WriteLine(users.Count);
        return View(users);
    }

    /*
    [HttpGet("Cart")]
    public async Task<ActionResult> GetCartAsync(CancellationToken token, [FromQuery] string promoCodeKey = "")
    {
        var userMail = User.Identity?.Name;
        var cartAndPromoCodeFront = new CartAndPromoCodeFront()
        {
            Cart = await _customerCart.GetAllAsync(userMail, token) ?? new List<Cart>(),
            PromoCode = await _promoCodes.GetOneAsync(promoCodeKey, token)
        };
        return View(cartAndPromoCodeFront);
    }

    [HttpPost("Cart")]
    public async Task<ActionResult> AddCartAsync(CancellationToken token,
        [FromForm] [Required] string phoneSlug,
        [FromForm] [Required] int amount)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("phoneSlug or amount not set");
        }

        var userMail = User.Identity?.Name;
        await _customerCart.AddOrUpdateAsync(phoneSlug, userMail, amount, token);

        return RedirectToAction("GetCart", "CustomerCart");
    }

    [HttpGet("Cart/Remove/{phoneSlug}")]
    public async Task<ActionResult> RemoveCartAsync(CancellationToken token,
        [FromRoute] [Required] string phoneSlug)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("phoneSlug not set");
        }

        var userMail = User.Identity?.Name;
        await _customerCart.RemoveAsync(phoneSlug, userMail, token);

        return RedirectToAction("GetCart", "CustomerCart");
    }

    [HttpGet("Cart/Buy")]
    public async Task<ActionResult> BuyCartAsync(CancellationToken token, [FromQuery] string promoCodeKey = "")
    {
        var userMail = User.Identity?.Name;
        var carts = await _customerCart.BuyAsync(userMail, token);
        var totalSum = await _promoCodes.Buy(carts, promoCodeKey, token);

        return Ok($"BuyPhones. Total sum {totalSum}");
    }*/
}