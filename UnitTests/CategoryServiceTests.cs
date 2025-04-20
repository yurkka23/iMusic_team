using AutoMapper;
using iMusic.BL.Exceptions;
using iMusic.BL.Interfaces;
using iMusic.BL.Services;
using iMusic.DAL.Abstractions;
using iMusic.DAL.DTOs.Category;
using iMusic.DAL.Entities;
using iMusic.DAL.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests;

public class CategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<IOptionsSnapshot<HostSettings>> _hostSettingsMock;
    private readonly CategoryService _categoryService;
    private readonly HostSettings _hostSettings;

    public CategoryServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _fileServiceMock = new Mock<IFileService>();
        _hostSettingsMock = new Mock<IOptionsSnapshot<HostSettings>>();
        _hostSettings = new HostSettings { CurrentHost = "http://localhost" };
        _hostSettingsMock.Setup(s => s.Value).Returns(_hostSettings);
        _categoryService = new CategoryService(_unitOfWorkMock.Object, _mapperMock.Object, _fileServiceMock.Object, _hostSettingsMock.Object);
    }

    [Fact]
    public async Task CreateCategoryAsync_ValidInput_CreatesCategoryAndReturnsTrue()
    {
        // Arrange
        var categoryName = "Pop";
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("pop.jpg");
        var filePath = "/images/pop.jpg";
        var newCategory = new Category { Name = categoryName, CategoryImgUrl = filePath, CreatedTime = DateTimeOffset.Now };

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Name == categoryName, default))
            .ReturnsAsync((Category)null);
        _fileServiceMock.Setup(fs => fs.GetFilePathAsync(mockFile.Object))
            .ReturnsAsync(filePath);
        _unitOfWorkMock.Setup(uow => uow.Categories.InsertAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _categoryService.CreateCategoryAsync(mockFile.Object, categoryName);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Name == categoryName, default), Times.Once);
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(mockFile.Object), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.InsertAsync(It.Is<Category>(c => c.Name == categoryName && c.CategoryImgUrl == filePath)), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateCategoryAsync_CategoryNameExists_ThrowsBadRequestException()
    {
        // Arrange
        var categoryName = "Pop";
        var mockFile = new Mock<IFormFile>();
        var existingCategory = new Category { Name = categoryName };

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Name == categoryName, default))
            .ReturnsAsync(existingCategory);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _categoryService.CreateCategoryAsync(mockFile.Object, categoryName));
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Categories.InsertAsync(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateCategoryAsync_NoFile_ThrowsNoFileException()
    {
        // Arrange
        var categoryName = "Pop";

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Name == categoryName, default))
            .ReturnsAsync((Category)null);

        // Act & Assert
        await Assert.ThrowsAsync<NoFileException>(() => _categoryService.CreateCategoryAsync(null, categoryName));
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Categories.InsertAsync(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task EditCategoryAsync_ValidInputWithNewImage_UpdatesCategoryAndReturnsTrue()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categoryName = "Rock";
        var newCategoryName = "Hard Rock";
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("hardrock.jpg");
        var filePath = "/images/hardrock.jpg";
        var existingCategory = new Category { Id = categoryId, Name = categoryName, CategoryImgUrl = "/images/rock.jpg" };

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == categoryId, default))
            .ReturnsAsync(existingCategory);
        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Name == newCategoryName, default))
            .ReturnsAsync((Category)null);
        _fileServiceMock.Setup(fs => fs.GetFilePathAsync(mockFile.Object))
            .ReturnsAsync(filePath);
        _fileServiceMock.Setup(fs => fs.DeleteFile(existingCategory.CategoryImgUrl));
        //_unitOfWorkMock.Setup(uow => uow.Categories.Update(It.IsAny<Category>()))
        //    .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _categoryService.EditCategoryAsync(categoryId, mockFile.Object, newCategoryName);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == categoryId, default), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Name == newCategoryName, default), Times.Once);
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(mockFile.Object), Times.Once);
        _fileServiceMock.Verify(fs => fs.DeleteFile(existingCategory.CategoryImgUrl), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(It.Is<Category>(c => c.Id == categoryId && c.Name == newCategoryName && c.CategoryImgUrl == filePath)), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task EditCategoryAsync_ValidInputWithoutNewImage_UpdatesCategoryAndReturnsTrue()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categoryName = "Rock";
        var newCategoryName = "Hard Rock";
        var existingCategory = new Category { Id = categoryId, Name = categoryName, CategoryImgUrl = "/images/rock.jpg" };

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == categoryId, default))
            .ReturnsAsync(existingCategory);
        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Name == newCategoryName, default))
            .ReturnsAsync((Category)null);
        //_unitOfWorkMock.Setup(uow => uow.Categories.Update(It.IsAny<Category>()))
        //    .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _categoryService.EditCategoryAsync(categoryId, null, newCategoryName);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == categoryId, default), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Name == newCategoryName, default), Times.Once);
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _fileServiceMock.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(It.Is<Category>(c => c.Id == categoryId && c.Name == newCategoryName && c.CategoryImgUrl == "/images/rock.jpg")), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task EditCategoryAsync_CategoryNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categoryName = "Rock";
        var mockFile = new Mock<IFormFile>();

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == categoryId, default))
            .ReturnsAsync((Category)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _categoryService.EditCategoryAsync(categoryId, mockFile.Object, categoryName));
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == categoryId, default), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Name == categoryName, default), Times.Never);
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _fileServiceMock.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task EditCategoryAsync_NewCategoryNameExists_ThrowsBadRequestException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categoryName = "Rock";
        var newCategoryName = "Hard Rock";
        var mockFile = new Mock<IFormFile>();
        var existingCategory = new Category { Id = categoryId, Name = categoryName, CategoryImgUrl = "/images/rock.jpg" };
        var conflictingCategory = new Category { Name = newCategoryName };

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == categoryId, default))
            .ReturnsAsync(existingCategory);
        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Name == newCategoryName, default))
            .ReturnsAsync(conflictingCategory);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _categoryService.EditCategoryAsync(categoryId, mockFile.Object, newCategoryName));
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == categoryId, default), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Name == newCategoryName, default), Times.Once);
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _fileServiceMock.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Categories.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ValidId_DeletesCategoryAndReturnsTrue()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categoryToDelete = new Category { Id = categoryId, CategoryImgUrl = "/images/pop.jpg" };

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == categoryId, default))
            .ReturnsAsync(categoryToDelete);
        //_unitOfWorkMock.Setup(uow => uow.Categories.Delete(categoryToDelete))
        //    .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);
        _fileServiceMock.Setup(fs => fs.DeleteFile(categoryToDelete.CategoryImgUrl));

        // Act
        var result = await _categoryService.DeleteCategoryAsync(categoryId);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == categoryId, default), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.Delete(categoryToDelete), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
        _fileServiceMock.Verify(fs => fs.DeleteFile(categoryToDelete.CategoryImgUrl), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ValidIdWithoutImage_DeletesCategoryAndReturnsTrue()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categoryToDelete = new Category { Id = categoryId, CategoryImgUrl = null };

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == categoryId, default))
            .ReturnsAsync(categoryToDelete);
        //_unitOfWorkMock.Setup(uow => uow.Categories.Delete(categoryToDelete))
        //    .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);
        _fileServiceMock.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Never);

        // Act
        var result = await _categoryService.DeleteCategoryAsync(categoryId);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == categoryId, default), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Categories.Delete(categoryToDelete), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryAsync_CategoryNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == categoryId, default))
            .ReturnsAsync((Category)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _categoryService.DeleteCategoryAsync(categoryId));
        _unitOfWorkMock.Verify(uow => uow.Categories.Delete(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
        _fileServiceMock.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Never);
    }
}
