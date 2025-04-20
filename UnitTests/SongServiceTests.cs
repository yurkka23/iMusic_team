using AutoMapper;
using iMusic.BL.Exceptions;
using iMusic.BL.Interfaces;
using iMusic.BL.Services;
using iMusic.DAL.Abstractions;
using iMusic.DAL.DTOs.Search;
using iMusic.DAL.DTOs.Song;
using iMusic.DAL.Entities;
using iMusic.DAL.Entities.Enums;
using iMusic.DAL.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using NAudio.Wave;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests;

public class SongServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<IOptionsSnapshot<HostSettings>> _hostSettingsMock;
    private readonly SongService _songService;

    public SongServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _fileServiceMock = new Mock<IFileService>();
        _hostSettingsMock = new Mock<IOptionsSnapshot<HostSettings>>();
        _hostSettingsMock.Setup(s => s.Value).Returns(new HostSettings { CurrentHost = "http://localhost" });
        _songService = new SongService(_unitOfWorkMock.Object, _mapperMock.Object, _fileServiceMock.Object, _hostSettingsMock.Object);
    }


    [Fact]
    public async Task UploadSongAsync_InvalidAlbumId_ThrowsNotFoundException()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var songDTO = new UploadSongDTO { AlbumId = Guid.NewGuid() };

        _unitOfWorkMock.Setup(uow => uow.Albums.GetFirstOrDefaultAsync(x => x.Id == songDTO.AlbumId, default))
            .ReturnsAsync((Album)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _songService.UploadSongAsync(currentUserId, songDTO));
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Category, bool>>>(), default), Times.Never);
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _fileServiceMock.Verify(fs => fs.GetSongFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Songs.InsertAsync(It.IsAny<Song>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task UploadSongAsync_InvalidCategoryName_ThrowsBadRequestException()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var songDTO = new UploadSongDTO { CategoryName = "NonExistent" };

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Name == "NonExistent", default))
            .ReturnsAsync((Category)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _songService.UploadSongAsync(currentUserId, songDTO));
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _fileServiceMock.Verify(fs => fs.GetSongFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Songs.InsertAsync(It.IsAny<Song>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task UploadSongAsync_NoImageFile_ThrowsNoFileException()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var songDTO = new UploadSongDTO { CategoryName = "Pop", SongImg = null };
        var category = new Category { Id = Guid.NewGuid(), Name = "Pop" };

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Name == songDTO.CategoryName, default))
            .ReturnsAsync(category);

        // Act & Assert
        await Assert.ThrowsAsync<NoFileException>(() => _songService.UploadSongAsync(currentUserId, songDTO));
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(null), Times.Never);
        _fileServiceMock.Verify(fs => fs.GetSongFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Songs.InsertAsync(It.IsAny<Song>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task UploadSongAsync_NoSongFile_ThrowsNoFileException()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var songDTO = new UploadSongDTO { CategoryName = "Pop", SongFile = null };
        var category = new Category { Id = Guid.NewGuid(), Name = "Pop" };
        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Name == songDTO.CategoryName, default))
            .ReturnsAsync(category);
        _fileServiceMock.Setup(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync("/images/test.jpg");

        // Act & Assert
        await Assert.ThrowsAsync<NoFileException>(() => _songService.UploadSongAsync(currentUserId, songDTO));
        _fileServiceMock.Verify(fs => fs.GetSongFilePathAsync(null), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Songs.InsertAsync(It.IsAny<Song>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task EditSongAsync_ValidDTOWithNewFiles_UpdatesSong()
    {
        // Arrange
        var songId = Guid.NewGuid();
        var categoryName = "Rock";
        var category = new Category { Id = Guid.NewGuid(), Name = categoryName };
        var songDTO = new EditSongDTO
        {
            Id = songId,
            CategoryName = categoryName,
            SongImg = null,
            SongFile = null,
            Title = "Updated Song",
            Text = "New Lyrics",
            Status = Status.Private
        };
        var existingSong = new Song { Id = songId, SongImgUrl = "/images/old.jpg", SongUrl = "/songs/old.mp3" };
        var newImgPath = "/images/new.jpg";
        var newSongPath = "/songs/new.mp3";

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Name == songDTO.CategoryName, default))
            .ReturnsAsync(category);
        
        _unitOfWorkMock.Setup(uow => uow.Songs.GetFirstOrDefaultAsync(s => s.Id == songDTO.Id, default))
            .ReturnsAsync(existingSong);
        _fileServiceMock.Setup(fs => fs.DeleteFile(existingSong.SongImgUrl));
        _fileServiceMock.Setup(fs => fs.DeleteFile(existingSong.SongUrl));
        _fileServiceMock.Setup(fs => fs.GetFilePathAsync(songDTO.SongImg))
            .ReturnsAsync(newImgPath);
        _fileServiceMock.Setup(fs => fs.GetSongFilePathAsync(songDTO.SongFile))
            .ReturnsAsync(newSongPath);
        _unitOfWorkMock.Setup(uow => uow.Songs.Update(It.IsAny<Song>()));
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);
        //_fileServiceMock.Setup(fs => fs.GetStreamFromFileAsync(songDTO.SongFile))
        //    .ReturnsAsync(new MemoryStream(new byte[] { 0x00 }));

        // Act
        await _songService.EditSongAsync(songDTO);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Name == songDTO.CategoryName, default), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Songs.GetFirstOrDefaultAsync(s => s.Id == songDTO.Id, default), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Songs.Update(It.Is<Song>(s =>
            s.CategoryId == category.Id &&
            s.Title == songDTO.Title &&
            s.Text == songDTO.Text &&
            s.Status == songDTO.Status)), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

}
