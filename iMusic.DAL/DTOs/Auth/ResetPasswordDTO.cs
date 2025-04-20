using System.ComponentModel.DataAnnotations;

namespace iMusic.DAL.DTOs.Auth;

public class ResetPasswordDTO
{
    public string? NewPassword { get; set; }
    public string? ConfirmNewPassword { get; set; }
    public string? OldPassword { get; set; }
}
