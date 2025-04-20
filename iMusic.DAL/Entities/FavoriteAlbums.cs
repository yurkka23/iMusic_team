
namespace iMusic.DAL.Entities;

public class FavoriteAlbums
{
    public DateTimeOffset AddedTime { get; set; }

    //relations
    public Guid AlbumId { get; set; }
    public Album Album { get; set; }
    public Guid FavoritelistId { get; set; }
    public FavoriteList Favoritelist { get; set; }
}
