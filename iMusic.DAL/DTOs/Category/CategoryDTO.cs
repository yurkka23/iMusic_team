namespace iMusic.DAL.DTOs.Category;

public class CategoryDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryImgUrl { get; set; } = string.Empty;
    public DateTimeOffset CreatedTime { get; set; }
}
