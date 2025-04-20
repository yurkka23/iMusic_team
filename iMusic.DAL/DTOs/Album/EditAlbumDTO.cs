using iMusic.DAL.Entities.Enums;
using Microsoft.AspNetCore.Http;

namespace iMusic.DAL.DTOs.Album;

public class EditAlbumDTO 
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Status Status { get; set; }
    public Guid CategoryId { get; set; }
    public IFormFile? AlbumImg { get; set; }
}

