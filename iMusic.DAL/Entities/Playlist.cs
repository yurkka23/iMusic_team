using iMusic.DAL.Entities.Enums;

namespace iMusic.DAL.Entities;

public class Playlist
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PlaylistImgUrl { get; set; }
    public Status Status { get; set; }
    public int CountRate { get; set; } //+1 if user add or like 

    public DateTimeOffset CreatedTime { get; set; }

    //relations
    public Guid AuthorId { get; set; }
    public User User { get; set; }
    public ICollection<SongPlaylist>? SongPlaylists { get; set; }
    public ICollection<FavoritePlaylists>? FavoritePlaylists { get; set; }

}
