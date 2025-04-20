using iMusic.DAL.Entities.Enums;
using Microsoft.AspNetCore.Http;

namespace iMusic.DAL.DTOs.Playlist;

public class CreatePlayListDTO
{
    public string Title { get; set; } = string.Empty;
    public Status Status { get; set; }
    public IFormFile PlayListImg { get; set; }

}
