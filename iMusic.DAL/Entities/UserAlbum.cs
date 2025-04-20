namespace iMusic.DAL.Entities;

public class UserAlbum
{
    public DateTimeOffset AddedTime { get; set; }

    //relations
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid AlbumId { get; set; }
    public Album Album { get; set; }
}
