using iMusic.DAL.Entities;

namespace iMusic.DAL.Abstractions;

public interface IUnitOfWork
{
    IRepositoryAsync<User> Users { get; }
    IRepositoryAsync<Category> Categories { get; }
    IRepositoryAsync<FavoriteList> FavoriteLists { get; }
    IRepositoryAsync<Song> Songs { get; }
    IRepositoryAsync<Album> Albums { get; }
    IRepositoryAsync<Playlist> Playlists { get; }
    IRepositoryAsync<UserSong> UserSongs { get; }
    IRepositoryAsync<UserAlbum> UserAlbums { get; }
    IRepositoryAsync<SongPlaylist> SongPlaylists { get; }
    IRepositoryAsync<FavoritePlaylists> FavoritePlaylists { get; }
    IRepositoryAsync<FavoriteAlbums> FavoriteAlbums { get; }
    IRepositoryAsync<FavoriteSongs> FavoriteSongs { get; }

    Task<bool> SaveAsync();
}
