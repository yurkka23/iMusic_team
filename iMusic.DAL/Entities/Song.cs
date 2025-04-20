using iMusic.DAL.Entities.Enums;

namespace iMusic.DAL.Entities;

public class Song
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Text { get; set; } 
    public string SongUrl {  get; set; } = string.Empty;
    public double Duration { get; set; }
    public int CountRate { get; set; } //+1 if user add or like this song
    public string? SongImgUrl { get; set; }
    public Status Status { get; set; }
    public DateTimeOffset CreatedTime { get; set; }

    //relations
    public Guid SingerId { get; set; }
    public User Singer { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    public Guid? AlbumId { get; set; }
    public Album? Album { get; set; }
    public ICollection<FavoriteSongs>? FavoriteSongs { get; set; }
    public ICollection<SongPlaylist>? SongPlaylists { get; set; }
    public ICollection<UserSong>? UserSongs { get; set; }
}
