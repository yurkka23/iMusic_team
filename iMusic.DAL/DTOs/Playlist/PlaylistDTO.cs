using iMusic.DAL.DTOs.Song;
using iMusic.DAL.DTOs.User;
using iMusic.DAL.Entities.Enums;

namespace iMusic.DAL.DTOs.Playlist;

public class PlaylistDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PlaylistImgUrl { get; set; }
    public Status Status { get; set; }
    public DateTimeOffset CreatedTime { get; set; }
    public int CountRate { get; set; }

    //relations
    public UserDTO? Author { get; set; } = new UserDTO();
    public ICollection<SongDTO>? Songs { get; set; } = new List<SongDTO>();
   
}
