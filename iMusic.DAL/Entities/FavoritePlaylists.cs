namespace iMusic.DAL.Entities;

public class FavoritePlaylists
{
    public DateTimeOffset AddedTime { get; set; }

    //relations
    public Guid PlaylistId { get; set; }
    public Playlist Playlist { get; set; }
    public Guid FavoritelistId { get; set; }
    public FavoriteList Favoritelist { get; set; }
}
