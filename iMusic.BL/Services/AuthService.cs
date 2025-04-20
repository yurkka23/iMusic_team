using iMusic.BL.Interfaces;
using iMusic.DAL.Constants;
using iMusic.DAL.DTOs.Auth;
using iMusic.DAL.Entities;
using iMusic.DAL.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using IdentityModel;
using iMusic.BL.Exceptions;

namespace iMusic.BL.Services;

public class AuthService : IAuthService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<AppRole> _roleManager;
    public AuthService(IOptionsSnapshot<JwtSettings> jwtSettings, UserManager<User> userManager,
        SignInManager<User> signInManager, RoleManager<AppRole> roleManager, ITokenService tokenService)
    {
        _jwtSettings = jwtSettings.Value;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }
    public async Task<AuthenticatedResponseDTO> Login(LoginDTO userModel)
    {
        var user = await _userManager.FindByEmailAsync(userModel.Email);

        if (user.IsBanned)
        {
            throw new BadRequestException("You are banned");
        }

        if (user is null)
        {
            throw new BadRequestException("Incorrect login");
        }

        var result = await _signInManager.PasswordSignInAsync(user, userModel.Password, false, false);

        if (!result.Succeeded)
        {
            throw new BadRequestException("Incorrect password");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var claims = await _userManager.GetClaimsAsync(user);
        var accessToken = _tokenService.GenerateAccessTokenTokenAsync(claims);
        var refreshToken = _tokenService.GenerateRefreshTokenAsync();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpires = DateTimeOffset.Now.AddMinutes(_jwtSettings.RefreshInMinutes);

        await _userManager.UpdateAsync(user);

        return new AuthenticatedResponseDTO { AccessToken = accessToken, RefreshToken = refreshToken };
    }
    public async Task<IdentityResult> Register(RegisterDTO userModel)
    {
        var user = new User
        {
            UserName = userModel.UserName,
            FirstName = userModel.FirstName,
            LastName = userModel.LastName,
            Email = userModel.Email,
            RegisterTime = DateTimeOffset.Now,
            UserImgUrl = null
        };

        var result = await _userManager.CreateAsync(user, userModel.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);

            throw new BadRequestException(string.Join(", ", errors));

        }

        var result1 = await _userManager.AddToRoleAsync(user, RoleConstants.UserRole);

        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(r => new Claim(JwtClaimTypes.Role, r));
        var claims = new List<Claim>()
            {
                new Claim(JwtClaimTypes.Name, user.UserName),
                new Claim(JwtClaimTypes.Id, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName + ' ' + user.LastName)
            };
        claims.AddRange(roleClaims);

        result = await _userManager.AddClaimsAsync(user, claims);
        return result;
    }

    public async Task ResetPasswordAsync(ResetPasswordDTO model, Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            throw new BadRequestException("Invalid Request");

        var resetPassResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

        if (!resetPassResult.Succeeded)
        {
            var errors = resetPassResult.Errors.Select(e => e.Description);

            throw new BadRequestException(string.Join(", ", errors));
        }

    }
}
