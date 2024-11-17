using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinAlert.Identity.Core.Domain;
using FinAlert.StockAlertApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FinAlert.StockAlertApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;

    public AccountController(ILogger<AccountController> logger, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest("User with this email already exists");
            if (model.Password != model.ConfirmPassword)
                return BadRequest("Passwords do not match");

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                return Ok("User created successfully");
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

    [HttpPost("changepassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return BadRequest("Invalid email or password");
            if (!await _userManager.CheckPasswordAsync(user, model.OldPassword))
                return BadRequest("Invalid email or password");
            if (model.NewPassword != model.ConfirmNewPassword)
                return BadRequest("Passwords do not match");

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password changed successfully");
            }
            else
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");

            return StatusCode(500);
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return BadRequest("Invalid email or password");
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return BadRequest("Invalid email or password");

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return Ok("Logged in successfully");
            }
            else
            {
                return generateSignInResponse(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing in user");

            return StatusCode(500);
        }
    }

    [AllowAnonymous]
    [HttpPost("token")]
    public async Task<IActionResult> GetToken([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return BadRequest("Invalid email or password");
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return BadRequest("Invalid email or password");

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var token = createJwtToken(user);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            else
            {
                return generateSignInResponse(result);
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

    #region private methods

    private JwtSecurityToken createJwtToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ArgumentNullException.ThrowIfNullOrEmpty(user.Email, nameof(user));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return token;
    }

    private IActionResult generateSignInResponse(Microsoft.AspNetCore.Identity.SignInResult result)
    {
        if (result.IsLockedOut)
            return BadRequest("User account is locked out.");
        if (result.IsNotAllowed)
            return BadRequest("User is not allowed to sign in.");
        if (result.RequiresTwoFactor)
            return BadRequest("Two-factor authentication is required.");

        return BadRequest("Invalid login attempt.");
    }
    #endregion
}