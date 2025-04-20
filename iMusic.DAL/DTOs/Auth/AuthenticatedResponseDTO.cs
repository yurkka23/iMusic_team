namespace iMusic.DAL.DTOs.Auth;

public class AuthenticatedResponseDTO
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
