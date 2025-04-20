namespace iMusic.DAL.DTOs.Auth;
public class RegisterDTO
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; }
}