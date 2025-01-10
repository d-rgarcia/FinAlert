using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinAlert.Identity.Core.Domain;
using FinAlert.StockAlertApi.Models;
using FinAlert.StockAlertApi.Models.HttpResponse;
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
            return BadRequest(modelValidationErrors());

        try
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return Conflict(ResponseResult.Failure("User with this email already exists"));
            if (model.Password != model.ConfirmPassword)
                return BadRequest(ResponseResult.Failure("Passwords do not match"));

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                return Ok(ResponseResult.Success());
            }
            else
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");

            return StatusCode(500, ResponseResult.Failure("Error registering user"));
        }
    }

    [HttpPost("changepassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(modelValidationErrors());

        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return Unauthorized(ResponseResult.Failure("Invalid email or password"));
            if (!await _userManager.CheckPasswordAsync(user, model.OldPassword))
                return Unauthorized(ResponseResult.Failure("Invalid email or password"));
            if (model.NewPassword != model.ConfirmNewPassword)
                return BadRequest(ResponseResult.Failure("Passwords do not match"));

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(ResponseResult.Success());
            }
            else
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");

            return StatusCode(500, ResponseResult.Failure("Error changing password"));
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(modelValidationErrors());

        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return BadRequest(ResponseResult.Failure("Invalid email or password"));
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Ok(ResponseResult.Success());
            }
            else
            {
                return BadRequest(generateSignInResponse(result));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, ResponseResult.Failure("An unexpected error occurred"));
        }
    }

    [AllowAnonymous]
    [HttpPost("token")]
    public async Task<IActionResult> GetToken([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(modelValidationErrors());

        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized(ResponseResult.Failure("Invalid email or password"));

            var token = createJwtToken(user);

            return Ok(ResponseResult.Success(new { token = new JwtSecurityTokenHandler().WriteToken(token) }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating token");
            return StatusCode(500, ResponseResult.Failure("Error generating token"));
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _signInManager.SignOutAsync();

            return Ok(ResponseResult.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing out user");

            return StatusCode(500, ResponseResult.Failure("Error while loging out"));
        }
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
            expires: DateTime.Now.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiresInMinutes")),
            signingCredentials: credentials);

        return token;
    }

    private ResponseResult generateSignInResponse(Microsoft.AspNetCore.Identity.SignInResult result)
    {
        if (result.IsLockedOut)
            return ResponseResult.Failure("User account is locked out.");
        if (result.IsNotAllowed)
            return ResponseResult.Failure("User is not allowed to sign in.");
        if (result.RequiresTwoFactor)
            return ResponseResult.Failure("Two-factor authentication is required.");

        return ResponseResult.Failure("Invalid login attempt.");
    }

    private ResponseResult modelValidationErrors()
    {
        var validationErrors = ModelState.Values
            .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
            .ToList();

        return ResponseResult.Failure(validationErrors);
    }
    #endregion
}