
namespace iMusic.DAL.DTOs.User;

public class UserDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? AboutMe { get; set; }
    public string? UserImgUrl { get; set; } = string.Empty;
    public bool? IsBanned { get; set; }
    public bool? WantToBeSinger { get; set; }
    public DateTimeOffset? BannedTime { get; set; }
    public DateTimeOffset RegisterTime { get; set; }
    public ICollection<string>? UserRoles { get; set; }
}
