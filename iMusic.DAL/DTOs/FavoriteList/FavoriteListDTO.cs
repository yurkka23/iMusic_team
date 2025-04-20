using iMusic.DAL.DTOs.Album;
using iMusic.DAL.DTOs.Playlist;
using iMusic.DAL.DTOs.Song;

namespace iMusic.DAL.DTOs.FavoriteList;

public class FavoriteListDTO
{
    public Guid Id { get; set; }
    public ICollection<SongDTO>? Songs { get; set; } = new List<SongDTO>();
    public ICollection<PlaylistDTO>? Playlists { get; set; } = new List<PlaylistDTO>();
    public ICollection<AlbumDTO>? Albums { get; set; } = new List<AlbumDTO>();
}
