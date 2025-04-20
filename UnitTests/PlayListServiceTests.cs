using AutoMapper;
using iMusic.BL.Exceptions;
using iMusic.BL.Interfaces;
using iMusic.BL.Services;
using iMusic.DAL.Abstractions;
using iMusic.DAL.DTOs.Playlist;
using iMusic.DAL.DTOs.Search;
using iMusic.DAL.Entities;
using iMusic.DAL.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests;

public class PlayListServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ISongService> _songServiceMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly PlayListService _playListService;

    public PlayListServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _songServiceMock = new Mock<ISongService>();
        _fileServiceMock = new Mock<IFileService>();
        _playListService = new PlayListService(_unitOfWorkMock.Object, _mapperMock.Object, _songServiceMock.Object, _fileServiceMock.Object);
    }

    [Fact]
    public async Task CreatePlayListAsync_ValidInput_CreatesPlaylistAndReturnsTrue()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var createPlaylistDto = new CreatePlayListDTO { Title = "My Playlist", Status = Status.Public, PlayListImg = new Mock<IFormFile>().Object };
        var imgPath = "/images/playlist.jpg";
        var newPlaylistId = Guid.NewGuid();

        _fileServiceMock.Setup(fs => fs.GetFilePathAsync(createPlaylistDto.PlayListImg))
            .ReturnsAsync(imgPath);
        _unitOfWorkMock.Setup(uow => uow.Playlists.InsertAsync(It.IsAny<Playlist>()))
            .Callback<Playlist>(p => p.Id = newPlaylistId);
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _playListService.CreatePlayListAsync(currentUserId, createPlaylistDto);

        // Assert
        Assert.True(result);
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(createPlaylistDto.PlayListImg), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Playlists.InsertAsync(It.Is<Playlist>(p =>
            p.Title == createPlaylistDto.Title &&
            p.Status == createPlaylistDto.Status &&
            p.AuthorId == currentUserId &&
            p.PlaylistImgUrl == imgPath)), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task CreatePlayListAsync_NoImageFile_ThrowsNoFileException()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var createPlaylistDto = new CreatePlayListDTO { Title = "My Playlist", Status = Status.Public, PlayListImg = null };

        // Act & Assert
        await Assert.ThrowsAsync<NoFileException>(() => _playListService.CreatePlayListAsync(currentUserId, createPlaylistDto));
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Playlists.InsertAsync(It.IsAny<Playlist>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task DeletePlaylistAsync_ValidId_DeletesPlaylistAndRelatedEntities()
    {
        // Arrange
        var playlistId = Guid.NewGuid();
        var playlistToDelete = new Playlist { Id = playlistId, PlaylistImgUrl = "/images/playlist.jpg" };
        var favoritePlaylists = new List<FavoritePlaylists> { new FavoritePlaylists { PlaylistId = playlistId } };
        var songPlaylists = new List<SongPlaylist> { new SongPlaylist { PlaylistId = playlistId } };

        _unitOfWorkMock.Setup(uow => uow.Playlists.GetFirstOrDefaultAsync(p => p.Id == playlistId, default))
            .ReturnsAsync(playlistToDelete);
        _unitOfWorkMock.Setup(uow => uow.FavoritePlaylists.GetAsync(x => x.PlaylistId == playlistId, default, default, 0, int.MaxValue))
            .ReturnsAsync(favoritePlaylists);
        _unitOfWorkMock.Setup(uow => uow.SongPlaylists.GetAsync(x => x.PlaylistId == playlistId, default, default, 0, int.MaxValue))
            .ReturnsAsync(songPlaylists);
        _unitOfWorkMock.Setup(uow => uow.FavoritePlaylists.Delete(favoritePlaylists));
        _unitOfWorkMock.Setup(uow => uow.SongPlaylists.Delete(songPlaylists));
        _unitOfWorkMock.Setup(uow => uow.Playlists.Delete(playlistToDelete));
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _playListService.DeletePlaylistAsync(playlistId);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(uow => uow.Playlists.GetFirstOrDefaultAsync(p => p.Id == playlistId, default), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.FavoritePlaylists.GetAsync(x => x.PlaylistId == playlistId, default, default, 0, int.MaxValue), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SongPlaylists.GetAsync(x => x.PlaylistId == playlistId, default, default, 0, int.MaxValue), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.FavoritePlaylists.Delete(favoritePlaylists), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SongPlaylists.Delete(songPlaylists), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Playlists.Delete(playlistToDelete), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletePlaylistAsync_InvalidId_ThrowsNotFoundException()
    {
        // Arrange
        var playlistId = Guid.NewGuid();
        _unitOfWorkMock.Setup(uow => uow.Playlists.GetFirstOrDefaultAsync(p => p.Id == playlistId, default))
            .ReturnsAsync((Playlist)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _playListService.DeletePlaylistAsync(playlistId));
        _unitOfWorkMock.Verify(uow => uow.FavoritePlaylists.GetAsync(It.IsAny<Expression<Func<FavoritePlaylists, bool>>>(), default, default, 0, int.MaxValue), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SongPlaylists.GetAsync(It.IsAny<Expression<Func<SongPlaylist, bool>>>(), default, default, 0, int.MaxValue), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Playlists.Delete(It.IsAny<Playlist>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task EditPlayListAsync_ValidInputWithNewImage_UpdatesPlaylistAndReturnsTrue()
    {
        // Arrange
        var editPlaylistDto = new EditPlayListDTO { Id = Guid.NewGuid(), Title = "Updated Title", Status = Status.Private, PlaylistImg = new Mock<IFormFile>().Object };
        var existingPlaylist = new Playlist { Id = editPlaylistDto.Id, Title = "Old Title", Status = Status.Public, PlaylistImgUrl = "/images/old.jpg" };
        var newImagePath = "/images/new.jpg";

        _unitOfWorkMock.Setup(uow => uow.Playlists.GetFirstOrDefaultAsync(p => p.Id == editPlaylistDto.Id, default))
            .ReturnsAsync(existingPlaylist);
        _fileServiceMock.Setup(fs => fs.GetFilePathAsync(editPlaylistDto.PlaylistImg))
            .ReturnsAsync(newImagePath);
        _fileServiceMock.Setup(fs => fs.DeleteFile(existingPlaylist.PlaylistImgUrl));
        //_unitOfWorkMock.Setup(uow => uow.Playlists.Update(It.IsAny<Playlist>()))
        //    .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _playListService.EditPlayListAsync(editPlaylistDto);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(uow => uow.Playlists.GetFirstOrDefaultAsync(p => p.Id == editPlaylistDto.Id, default), Times.Once);
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(editPlaylistDto.PlaylistImg), Times.Once);
        _fileServiceMock.Verify(fs => fs.DeleteFile(existingPlaylist.PlaylistImgUrl), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Playlists.Update(It.Is<Playlist>(p =>
            p.Id == editPlaylistDto.Id &&
            p.Title == editPlaylistDto.Title &&
            p.Status == editPlaylistDto.Status &&
            p.PlaylistImgUrl == newImagePath)), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task EditPlayListAsync_ValidInputWithoutNewImage_UpdatesPlaylistAndReturnsTrue()
    {
        // Arrange
        var editPlaylistDto = new EditPlayListDTO { Id = Guid.NewGuid(), Title = "Updated Title", Status = Status.Private, PlaylistImg = null };
        var existingPlaylist = new Playlist { Id = editPlaylistDto.Id, Title = "Old Title", Status = Status.Public, PlaylistImgUrl = "/images/old.jpg" };

        _unitOfWorkMock.Setup(uow => uow.Playlists.GetFirstOrDefaultAsync(p => p.Id == editPlaylistDto.Id, default))
            .ReturnsAsync(existingPlaylist);
        //_unitOfWorkMock.Setup(uow => uow.Playlists.Update(It.IsAny<Playlist>()))
        //    .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _playListService.EditPlayListAsync(editPlaylistDto);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(uow => uow.Playlists.GetFirstOrDefaultAsync(p => p.Id == editPlaylistDto.Id, default), Times.Once);
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _fileServiceMock.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Playlists.Update(It.Is<Playlist>(p =>
            p.Id == editPlaylistDto.Id &&
            p.Title == editPlaylistDto.Title &&
            p.Status == editPlaylistDto.Status &&
            p.PlaylistImgUrl == "/images/old.jpg")), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task EditPlayListAsync_InvalidId_ThrowsNotFoundException()
    {
        // Arrange
        var editPlaylistDto = new EditPlayListDTO { Id = Guid.NewGuid(), Title = "Updated Title", Status = Status.Private, PlaylistImg = new Mock<IFormFile>().Object };
        _unitOfWorkMock.Setup(uow => uow.Playlists.GetFirstOrDefaultAsync(p => p.Id == editPlaylistDto.Id, default))
            .ReturnsAsync((Playlist)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _playListService.EditPlayListAsync(editPlaylistDto));
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _fileServiceMock.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Playlists.Update(It.IsAny<Playlist>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task ChangeImageAsync_ValidInput_UpdatesPlaylistImageAndReturnsTrue()
    {
        // Arrange
        var changePlaylistImageDto = new ChangePlaylistImageDTO { Id = Guid.NewGuid(), PlayListImg = new Mock<IFormFile>().Object };
        var existingPlaylist = new Playlist { Id = changePlaylistImageDto.Id, PlaylistImgUrl = "/images/old.jpg" };
        var newImagePath = "/images/new.jpg";

        _unitOfWorkMock.Setup(uow => uow.Playlists.GetFirstOrDefaultAsync(p => p.Id == changePlaylistImageDto.Id, default))
            .ReturnsAsync(existingPlaylist);
        _fileServiceMock.Setup(fs => fs.GetFilePathAsync(changePlaylistImageDto.PlayListImg))
            .ReturnsAsync(newImagePath);
        _fileServiceMock.Setup(fs => fs.DeleteFile(existingPlaylist.PlaylistImgUrl));
        //_unitOfWorkMock.Setup(uow => uow.Playlists.Update(It.IsAny<Playlist>()))
        //    .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _playListService.ChangeImageAsync(changePlaylistImageDto);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(uow => uow.Playlists.GetFirstOrDefaultAsync(p => p.Id == changePlaylistImageDto.Id, default), Times.Once);
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(changePlaylistImageDto.PlayListImg), Times.Once);
        _fileServiceMock.Verify(fs => fs.DeleteFile(existingPlaylist.PlaylistImgUrl), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Playlists.Update(It.Is<Playlist>(p => p.Id == changePlaylistImageDto.Id && p.PlaylistImgUrl == newImagePath)), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task ChangeImageAsync_InvalidPlaylistId_ThrowsNotFoundException()
    {
        // Arrange
        var changePlaylistImageDto = new ChangePlaylistImageDTO { Id = Guid.NewGuid(), PlayListImg = new Mock<IFormFile>().Object };
        _unitOfWorkMock.Setup(uow => uow.Playlists.GetFirstOrDefaultAsync(p => p.Id == changePlaylistImageDto.Id, default))
            .ReturnsAsync((Playlist)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _playListService.ChangeImageAsync(changePlaylistImageDto));
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _fileServiceMock.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Playlists.Update(It.IsAny<Playlist>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }
}
