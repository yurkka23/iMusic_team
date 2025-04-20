using iMusic.DAL.Abstractions;
using iMusic.DAL.DataContext;
using iMusic.DAL.Entities;

namespace iMusic.DAL.Realizations;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    private BaseRepositoryAsync<User> _users;
    private BaseRepositoryAsync<Category> _сategories;
    private BaseRepositoryAsync<FavoriteList> _favoriteLists;
    private BaseRepositoryAsync<Song> _songs;
    private BaseRepositoryAsync<Album> _albums;
    private BaseRepositoryAsync<Playlist> _playlists;
    private BaseRepositoryAsync<UserSong> _userSongs;
    private BaseRepositoryAsync<UserAlbum> _userAlbums;
    private BaseRepositoryAsync<SongPlaylist> _songPlaylists;
    private BaseRepositoryAsync<FavoritePlaylists> _favoritePlaylists;
    private BaseRepositoryAsync<FavoriteAlbums> _favoriteAlbums;
    private BaseRepositoryAsync<FavoriteSongs> _favoriteSongs;
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }
    public IRepositoryAsync<User> Users
    {
        get
        {
            return _users ??= new BaseRepositoryAsync<User>(_context);
        }
    }
    public IRepositoryAsync<Category> Categories
    {
        get
        {
            return _сategories ??= new BaseRepositoryAsync<Category>(_context);
        }
    }
    public IRepositoryAsync<FavoriteList> FavoriteLists
    {
        get
        {
            return _favoriteLists ??= new BaseRepositoryAsync<FavoriteList>(_context);
        }
    }
    public IRepositoryAsync<Song> Songs
    {
        get
        {
            return _songs ??= new BaseRepositoryAsync<Song>(_context);
        }
    }
    public IRepositoryAsync<Album> Albums
    {
        get
        {
            return _albums ??= new BaseRepositoryAsync<Album>(_context);
        }
    }
    public IRepositoryAsync<Playlist> Playlists
    {
        get
        {
            return _playlists ??= new BaseRepositoryAsync<Playlist>(_context);
        }
    }
    public IRepositoryAsync<UserSong> UserSongs
    {
        get
        {
            return _userSongs ??= new BaseRepositoryAsync<UserSong>(_context);
        }
    }
    public IRepositoryAsync<UserAlbum> UserAlbums
    {
        get
        {
            return _userAlbums ??= new BaseRepositoryAsync<UserAlbum>(_context);
        }
    }
    public IRepositoryAsync<SongPlaylist> SongPlaylists
    {
        get
        {
            return _songPlaylists ??= new BaseRepositoryAsync<SongPlaylist>(_context);
        }
    }
    public IRepositoryAsync<FavoritePlaylists> FavoritePlaylists
    {
        get
        {
            return _favoritePlaylists ??= new BaseRepositoryAsync<FavoritePlaylists>(_context);
        }
    }
    public IRepositoryAsync<FavoriteAlbums> FavoriteAlbums
    {
        get
        {
            return _favoriteAlbums ??= new BaseRepositoryAsync<FavoriteAlbums>(_context);
        }
    }

    public IRepositoryAsync<FavoriteSongs> FavoriteSongs
    {
        get
        {
            return _favoriteSongs ??= new BaseRepositoryAsync<FavoriteSongs>(_context);
        }
    }

    public async Task<bool> SaveAsync()
    {
        var entities = _context.ChangeTracker.Entries();

        if (!entities.Any())
        {
            return Convert.ToBoolean(await _context.SaveChangesAsync());
        }
        try
        {
            return Convert.ToBoolean(await _context.SaveChangesAsync());
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.InnerException);
            return false;
        }
    }
}
