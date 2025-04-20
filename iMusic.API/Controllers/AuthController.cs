using iMusic.BL.Interfaces;
using iMusic.DAL.DTOs.Auth;
using iMusic.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace iMusic.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseController
{
    private readonly SignInManager<User> _signInManager;
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    public AuthController(SignInManager<User> signInManager, IAuthService authService, ITokenService tokenService)
    {
        _signInManager = signInManager;
        _authService = authService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO userModel)
    {
        try
        {
            var res = await _authService.Login(userModel);
            return Ok(res);
        }
        catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO userModel)
    {
        if (userModel is null)
        {
            return BadRequest("Invalid client request");
        }
        try
        {
            var result = await _authService.Register(userModel);

            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }  
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(TokenApiDTO tokenDTO)
    {
        if (tokenDTO is null)
            return BadRequest("Invalid client request");
        try
        {
            var res = await _tokenService.RefreshTokenAsync(tokenDTO);

            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _authService.ResetPasswordAsync(model, UserId);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogOut()
    { 
        await _signInManager.SignOutAsync();
        return Ok(); 
    }
}
