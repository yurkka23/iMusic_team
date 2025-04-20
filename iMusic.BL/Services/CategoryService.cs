using AutoMapper;
using iMusic.BL.Exceptions;
using iMusic.BL.Interfaces;
using iMusic.DAL.Abstractions;
using iMusic.DAL.DTOs.Category;
using iMusic.DAL.Entities;
using iMusic.DAL.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace iMusic.BL.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly HostSettings _hostSettings;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService,
         IOptionsSnapshot<HostSettings> hostSettings)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileService = fileService;
        _hostSettings = hostSettings.Value;
    }

    public async Task<bool> CreateCategoryAsync(IFormFile categoryImg, string categoryName)
    {
        var category = await _unitOfWork.Categories.GetFirstOrDefaultAsync(c => c.Name == categoryName);

        if (category != null) throw new BadRequestException("Such category name already exists!");

        if (categoryImg is null) throw new NoFileException($"File is required! Please add file");

        var img = await _fileService.GetFilePathAsync(categoryImg);

        var newCategory = new Category()
        {
            CategoryImgUrl = img,
            Name = categoryName,
            CreatedTime = DateTimeOffset.Now
        };

        await _unitOfWork.Categories.InsertAsync(newCategory);

        return await _unitOfWork.SaveAsync();

    }

    public async Task<bool> EditCategoryAsync(Guid id, IFormFile? categoryImg, string categoryName)
    {
        var category = await _unitOfWork.Categories.GetFirstOrDefaultAsync(c => c.Id == id);

        if (category == null) throw new NotFoundException("Such category doesn't exists! ");

        if(category.Name != categoryName)
        {
            var checkCategoryName = await _unitOfWork.Categories.GetFirstOrDefaultAsync(c => c.Name == categoryName);

            if (checkCategoryName != null) throw new BadRequestException("Such category already exists! ");

        }

        if (categoryImg is not null)
        {
            var img = await _fileService.GetFilePathAsync(categoryImg);

            if (category.CategoryImgUrl != null) _fileService.DeleteFile(category.CategoryImgUrl);

            category.CategoryImgUrl = img;

        }

        category.Name = categoryName;
       
        _unitOfWork.Categories.Update(category);

        return await _unitOfWork.SaveAsync();

    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        var category = await _unitOfWork.Categories.GetFirstOrDefaultAsync(c => c.Id == id);

        if (category == null) throw new NotFoundException("Such category doesn't exists! ");

        _unitOfWork.Categories.Delete(category);

        var res = await _unitOfWork.SaveAsync();

        if (res && category.CategoryImgUrl != null) _fileService.DeleteFile(category.CategoryImgUrl);
       
        return res;
    }

    public async Task<IEnumerable<CategoryDTO>> GetAllCatgories()
    {
        var categories =_mapper.Map<IEnumerable<CategoryDTO>>( await _unitOfWork.Categories.GetAsync(null, c => c.OrderBy(c => c.Name)));

        return categories;
    }

    public async Task<CategoryDTO> GetCategory(Guid id)
    {
        var category = _mapper.Map<CategoryDTO>(await _unitOfWork.Categories.GetFirstOrDefaultAsync(x => x.Id == id));

        return category;
    }
}
