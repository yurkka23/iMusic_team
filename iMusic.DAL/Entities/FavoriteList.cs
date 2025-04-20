namespace iMusic.DAL.Entities;

public class FavoriteList
{
    public Guid UserId { get; set; }
    public User User {  get; set; }
    public ICollection<FavoriteSongs>? Songs { get; set; }
    public ICollection<FavoritePlaylists>? Playlists { get; set; }
    public ICollection<FavoriteAlbums>? Albums { get; set; }

}
