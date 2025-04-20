
using iMusic.DAL.DTOs.Category;
using iMusic.DAL.Entities.Enums;

namespace iMusic.DAL.DTOs.Song;

public class SongDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Text { get; set; }
    public string SongUrl { get; set; } = string.Empty;
    public double Duration { get; set; }
    public int CountRate { get; set; }
    public string? SongImgUrl { get; set; }
    public Status Status { get; set; }
    public DateTimeOffset CreatedTime { get; set; }

    public Guid? SingerId { get; set; }
    public string? SingerFullName { get; set; }
    public string? SingerUserName { get; set; }

    public CategoryDTO? Category { get; set; }
    public Guid? AlbumId { get; set; }
    public string? AlbumTitle { get; set; }

}
