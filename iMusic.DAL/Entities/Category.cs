namespace iMusic.DAL.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryImgUrl { get; set; } = string.Empty;
    public DateTimeOffset CreatedTime { get; set; }

    //relations
    public ICollection<Song>? Songs { get; set; }
    public ICollection<Album>? Albums { get; set; }

}
