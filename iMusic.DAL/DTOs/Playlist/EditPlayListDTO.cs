using iMusic.DAL.Entities.Enums;
using Microsoft.AspNetCore.Http;

namespace iMusic.DAL.DTOs.Playlist;

public class EditPlayListDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Status Status { get; set; }
    public IFormFile? PlaylistImg { get; set; }

}
