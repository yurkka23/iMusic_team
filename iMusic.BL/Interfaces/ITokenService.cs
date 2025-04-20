
using iMusic.DAL.DTOs.Auth;
using System.Security.Claims;

namespace iMusic.BL.Interfaces;

public interface ITokenService
{
    Task<AuthenticatedResponseDTO> RefreshTokenAsync(TokenApiDTO tokenApiModel);
    Task RevokeTokenAsync(ClaimsPrincipal principal);
    string GenerateAccessTokenTokenAsync(IEnumerable<Claim> claims);
    string GenerateRefreshTokenAsync();
    ClaimsPrincipal GetPrincipalFromExpiredTokenAsync(string token);
}
