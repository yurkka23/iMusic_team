using System.ComponentModel.DataAnnotations;

namespace iMusic.DAL.DTOs.User;

public class UserChangeRoleDTO
{
    [Required]
    public string RoleName { get; set; }
    [Required]
    public string UserId { get; set; }
}
