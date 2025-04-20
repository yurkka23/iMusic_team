using Microsoft.AspNetCore.Identity;

namespace iMusic.DAL.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AboutMe { get; set; }
    public string? UserImgUrl { get; set; } = string.Empty;
    public bool IsBanned { get; set; }
    public DateTimeOffset? BannedTime { get; set; }
    public DateTimeOffset RegisterTime { get; set; }
    public bool? WantToBeSinger { get; set; }
    //Auth
    public string RefreshToken { get; set; } = string.Empty;
    public DateTimeOffset? RefreshTokenExpires { get; set; }

    //relations
    public Guid? FavoriteListId { get; set; }
    public FavoriteList? FavoriteList { get; set; }
    public ICollection<Song>? Songs { get; set; }
    public ICollection<Album>? Albums { get; set; }
    public ICollection<UserSong>? UserSongs { get; set; }
    public ICollection<Playlist>? Playlists { get; set; }
    public ICollection<UserAlbum>? UserAlbums { get; set; }
    public ICollection<AppUserRole> UserRoles { get; set; }


}
