using iMusic.DAL.DTOs.Category;
using iMusic.DAL.DTOs.Song;
using iMusic.DAL.DTOs.User;
using iMusic.DAL.Entities.Enums;

namespace iMusic.DAL.DTOs.Album;

public class AlbumDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTimeOffset CreatedTime { get; set; }
    public Status Status { get; set; }
    public int CountRate { get; set; }
    public string AlbumImgUrl { get; set; }
    //relations
    public ICollection<SongDTO>? Songs { get; set; } = new List<SongDTO>();
    public UserDTO? Singer { get; set; } = new UserDTO();
    public CategoryDTO? Category { get; set; } = new CategoryDTO();
}
