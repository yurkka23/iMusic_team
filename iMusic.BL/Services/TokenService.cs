using iMusic.BL.Interfaces;
using iMusic.DAL.DTOs.Auth;
using iMusic.DAL.Entities;
using iMusic.DAL.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace iMusic.BL.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<User> _userManager;
    public TokenService(IOptionsSnapshot<JwtSettings> jwtSettings, UserManager<User> userManager)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthenticatedResponseDTO> RefreshTokenAsync(TokenApiDTO tokenApiModel)
    {
        if (tokenApiModel is null)
            throw new Exception("Invalid client request");

        string accessToken = tokenApiModel.AccessToken;
        string refreshToken = tokenApiModel.RefreshToken;


        var principal = GetPrincipalFromExpiredTokenAsync(accessToken);
        var userEmail = principal.Identity.Name;
        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpires <= DateTimeOffset.Now)
            throw new Exception("Invalid client request");

        var newAccessToken = GenerateAccessTokenTokenAsync(principal.Claims);
        var newRefreshToken = GenerateRefreshTokenAsync();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpires = DateTimeOffset.Now.AddMinutes(_jwtSettings.RefreshInMinutes);
        await _userManager.UpdateAsync(user);

        return new AuthenticatedResponseDTO()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
    public async Task RevokeTokenAsync(ClaimsPrincipal principal)
    {
        var userEmail = principal.Identity.Name;
        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user == null) throw new Exception("Invalid client request");

        user.RefreshToken = null;

        await _userManager.UpdateAsync(user);
    }
    public string GenerateAccessTokenTokenAsync(IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokeOptions = new JwtSecurityToken(
            //issuer: _jwtSettings.Issuer,
            //audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: signinCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return tokenString;
    }
    public string GenerateRefreshTokenAsync()
    {
       return Guid.NewGuid().ToString();
    }
    public ClaimsPrincipal GetPrincipalFromExpiredTokenAsync(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}
