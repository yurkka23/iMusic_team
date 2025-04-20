using iMusic.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace iMusic.DAL.DataContext;

public class ApplicationDbContext : IdentityDbContext<User, AppRole, Guid>
{
    //public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<FavoriteList> FavoriteLists { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<UserSong> UserSongs { get; set; }
    public DbSet<UserAlbum> UserAlbums { get; set; }
    public DbSet<SongPlaylist> SongPlaylists { get; set; }
    public DbSet<FavoritePlaylists> FavoritePlaylists { get; set; }
    public DbSet<FavoriteAlbums> FavoriteAlbums { get; set; }
    public DbSet<FavoriteSongs> FavoriteSongs { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
