using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace iMusic.DAL.DTOs.Category;

public class CreateCategoryDTO
{
    public string Name { get; set; } = string.Empty;
    public IFormFile CategoryImg { get; set; }
}
