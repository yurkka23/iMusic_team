namespace iMusic.DAL.Entities;

public class SongPlaylist
{
    public DateTimeOffset AddedTime { get; set; }

    //relations
    public Guid SongId { get; set; }
    public Song Song { get; set; }
    public Guid PlaylistId { get;set;}
    public Playlist Playlist { get; set; }

}
