using iMusic.BL.Interfaces;
using iMusic.DAL.Constants;
using iMusic.DAL.DTOs.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iMusic.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : BaseController
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [Authorize(Roles = RoleConstants.AdminRole)]
    [HttpPost("create-category")]
    public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryDTO createCategory)
    {
       
        var result = await _categoryService.CreateCategoryAsync(createCategory.CategoryImg, createCategory.Name);

        return Ok(result);
    }

    [Authorize(Roles = RoleConstants.AdminRole)]
    [HttpPut("edit-category")]
    public async Task<IActionResult> EditCategory([FromForm] EditCategoryDTO editCategory)
    {

        var result = await _categoryService.EditCategoryAsync(editCategory.Id, editCategory.CategoryImg, editCategory.Name);

        return Ok(result);
    }

    [Authorize(Roles = RoleConstants.AdminRole)]
    [HttpDelete("delete-category")]
    public async Task<IActionResult> DeleteCategory([FromQuery] Guid id)
    {

        var result = await _categoryService.DeleteCategoryAsync(id);

        return Ok(result);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllCatedories()
    {
        var result = await _categoryService.GetAllCatgories();

        return Ok(result);
    }

    [HttpGet("get-category")]
    public async Task<IActionResult> GetCatedory(Guid id)
    {
        var result = await _categoryService.GetCategory(id);

        return Ok(result);
    }
}
