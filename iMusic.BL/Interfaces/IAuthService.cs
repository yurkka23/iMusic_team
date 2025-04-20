using iMusic.DAL.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace iMusic.BL.Interfaces;

public interface IAuthService
{
    Task<AuthenticatedResponseDTO> Login(LoginDTO userModel);
    Task<IdentityResult> Register(RegisterDTO userModel);
    Task ResetPasswordAsync(ResetPasswordDTO model, Guid userId);
    
}
