using FinAlert.Identity.Core.Domain;
using FinAlert.StockAlertApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinAlert.StockAlertApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(ILogger<AccountController> logger, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (await _userManager.FindByEmailAsync(model.Email) != null)
            return BadRequest("User with this email already exists");
        if (model.Password != model.ConfirmPassword)
            return BadRequest("Passwords do not match");

        var user = new User
        {
            UserName = model.Email,
            Email = model.Email        };
        try
        {
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            
            return StatusCode(500);
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user is null)
                return BadRequest("Invalid email or password");
            if(await _userManager.CheckPasswordAsync(user, model.Password))
                return BadRequest("Invalid email or password");

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
                return Ok();
            else
            {
                if(result.IsLockedOut)
                    return BadRequest("Account is locked out");
                else
                    return BadRequest("Invalid login");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing in user");

            return StatusCode(500);
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _signInManager.SignOutAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing out user");

            return StatusCode(500);
        }
        return Ok();
    }
}