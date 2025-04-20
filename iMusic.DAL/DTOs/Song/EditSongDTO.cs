using iMusic.DAL.Entities.Enums;
using Microsoft.AspNetCore.Http;

namespace iMusic.DAL.DTOs.Song;

public class EditSongDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Text { get; set; }
    public Status Status { get; set; }
    public string CategoryName { get; set; }
    public IFormFile? SongFile { get; set; }
    public IFormFile? SongImg { get; set; }
}
