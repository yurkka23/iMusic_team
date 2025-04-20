namespace iMusic.DAL.Entities;

public class FavoriteSongs
{
    public DateTimeOffset AddedTime { get; set; }

    //relations
    public Guid SongId { get; set; }
    public Song Song { get; set; }
    public Guid FavoritelistId { get; set; }
    public FavoriteList Favoritelist { get; set; }
}
