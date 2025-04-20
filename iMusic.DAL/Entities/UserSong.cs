namespace iMusic.DAL.Entities;

public class UserSong
{
    public DateTimeOffset AddedTime { get; set; }

    //relations
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid SongId { get; set; }
    public Song Song { get; set; }
}
