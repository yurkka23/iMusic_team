using Microsoft.AspNetCore.Http;

namespace iMusic.DAL.DTOs.Category;

public class EditCategoryDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IFormFile? CategoryImg { get; set; }
}
