using iMusic.DAL.Entities.Enums;

namespace iMusic.DAL.Entities;

public class Album
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTimeOffset CreatedTime { get; set; }
    public Status Status { get; set; }
    public int CountRate { get; set; } //+1 if user add or like 
    public string? AlbumImgUrl { get; set; }


    //relations
    public ICollection<Song>? Songs { get; set; }  
    public Guid SingerId { get; set; }
    public User User { get; set; }
    public ICollection<UserAlbum>? UserAlbums { get; set; }
    public ICollection<FavoriteAlbums>? FavoriteAlbums { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }

}
