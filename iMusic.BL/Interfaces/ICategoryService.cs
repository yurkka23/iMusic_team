using iMusic.DAL.DTOs.Category;
using Microsoft.AspNetCore.Http;

namespace iMusic.BL.Interfaces;

public interface ICategoryService
{
    Task<bool> CreateCategoryAsync(IFormFile categoryImg, string categoryName);
    Task<bool> EditCategoryAsync(Guid id, IFormFile categoryImg, string categoryName);
    Task<bool> DeleteCategoryAsync(Guid id);
    Task<IEnumerable<CategoryDTO>> GetAllCatgories();
    Task<CategoryDTO> GetCategory(Guid id);


}
