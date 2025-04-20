using Microsoft.AspNetCore.Http;

namespace iMusic.DAL.DTOs.Playlist;

public class ChangePlaylistImageDTO
{
    public Guid Id { get; set; }
    public IFormFile PlayListImg { get; set; }
}
