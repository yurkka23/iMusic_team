using AutoMapper;
using iMusic.BL.Exceptions;
using iMusic.BL.Interfaces;
using iMusic.BL.Services;
using iMusic.DAL.Abstractions;
using iMusic.DAL.DTOs.Album;
using iMusic.DAL.DTOs.Song;
using iMusic.DAL.Entities;
using iMusic.DAL.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace UnitTests;

public class AlbumServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ISongService> _songServiceMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly AlbumService _albumService;

    public AlbumServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _songServiceMock = new Mock<ISongService>();
        _fileServiceMock = new Mock<IFileService>();
        _albumService = new AlbumService(_unitOfWorkMock.Object, _mapperMock.Object, _songServiceMock.Object, _fileServiceMock.Object);
    }

    [Fact]
    public async Task CreateAlbumAsync_ValidDTO_ReturnsNewAlbumId()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var createAlbumDTO = new CreateAlbumDTO
        {
            CategoryId = Guid.NewGuid(),
            Title = "Test Album",
            Status = Status.Public,
            AlbumImg = new Mock<IFormFile>().Object
        };
        var category = new Category { Id = createAlbumDTO.CategoryId };
        var filePath = "/images/test.jpg";
        var newAlbumId = Guid.NewGuid();
        var newAlbum = new Album
        {
            Id = newAlbumId,
            Title = createAlbumDTO.Title,
            Status = createAlbumDTO.Status,
            CategoryId = createAlbumDTO.CategoryId,
            CreatedTime = DateTimeOffset.Now,
            SingerId = currentUserId,
            CountRate = 0,
            AlbumImgUrl = filePath
        };

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == createAlbumDTO.CategoryId, default))
            .ReturnsAsync(category);
        _fileServiceMock.Setup(fs => fs.GetFilePathAsync(createAlbumDTO.AlbumImg))
            .ReturnsAsync(filePath);
        _unitOfWorkMock.Setup(uow => uow.Albums.InsertAsync(It.IsAny<Album>()))
            .Callback<Album>(album => album.Id = newAlbumId);
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _albumService.CreateAlbumAsync(currentUserId, createAlbumDTO);

        // Assert
        Assert.Equal(newAlbumId, result);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == createAlbumDTO.CategoryId, default), Times.Once);
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(createAlbumDTO.AlbumImg), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Albums.InsertAsync(It.Is<Album>(a =>
            a.Title == createAlbumDTO.Title &&
            a.Status == createAlbumDTO.Status &&
            a.CategoryId == createAlbumDTO.CategoryId &&
            a.SingerId == currentUserId &&
            a.AlbumImgUrl == filePath)), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAlbumAsync_InvalidCategoryId_ThrowsNotFoundException()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var createAlbumDTO = new CreateAlbumDTO { CategoryId = Guid.NewGuid() };

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == createAlbumDTO.CategoryId, default))
            .ReturnsAsync((Category)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _albumService.CreateAlbumAsync(currentUserId, createAlbumDTO));
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == createAlbumDTO.CategoryId, default), Times.Once);
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Albums.InsertAsync(It.IsAny<Album>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAlbumAsync_NoImage_ThrowsNoFileException()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var createAlbumDTO = new CreateAlbumDTO
        {
            CategoryId = Guid.NewGuid(),
            Title = "Test Album",
            Status = Status.Public,
            AlbumImg = null
        };
        var category = new Category { Id = createAlbumDTO.CategoryId };

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == createAlbumDTO.CategoryId, default))
            .ReturnsAsync(category);

        // Act & Assert
        await Assert.ThrowsAsync<NoFileException>(() => _albumService.CreateAlbumAsync(currentUserId, createAlbumDTO));
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == createAlbumDTO.CategoryId, default), Times.Once);
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Albums.InsertAsync(It.IsAny<Album>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task EditAlbumAsync_ValidDTO_ReturnsTrue()
    {
        // Arrange
        var editAlbumDTO = new EditAlbumDTO
        {
            Id = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Title = "Updated Album",
            Status = Status.Private,
            AlbumImg = new Mock<IFormFile>().Object
        };
        var category = new Category { Id = editAlbumDTO.CategoryId };
        var existingAlbum = new Album { Id = editAlbumDTO.Id, AlbumImgUrl = "/images/old.jpg" };
        var newFilePath = "/images/new.jpg";

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == editAlbumDTO.CategoryId, default))
            .ReturnsAsync(category);
        _unitOfWorkMock.Setup(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == editAlbumDTO.Id, default))
            .ReturnsAsync(existingAlbum);
        _fileServiceMock.Setup(fs => fs.GetFilePathAsync(editAlbumDTO.AlbumImg))
            .ReturnsAsync(newFilePath);
        _fileServiceMock.Setup(fs => fs.DeleteFile(existingAlbum.AlbumImgUrl));
        _unitOfWorkMock.Setup(uow => uow.Albums.Update(It.IsAny<Album>()));
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _albumService.EditAlbumAsync(editAlbumDTO);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == editAlbumDTO.CategoryId, default), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == editAlbumDTO.Id, default), Times.Once);
        _fileServiceMock.Verify(fs => fs.DeleteFile(existingAlbum.AlbumImgUrl), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Albums.Update(It.Is<Album>(a =>
            a.Id == editAlbumDTO.Id &&
            a.Title == editAlbumDTO.Title &&
            a.Status == editAlbumDTO.Status &&
            a.CategoryId == editAlbumDTO.CategoryId &&
            a.AlbumImgUrl == newFilePath)), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task EditAlbumAsync_ValidDTO_NoNewImage_ReturnsTrue()
    {
        // Arrange
        var editAlbumDTO = new EditAlbumDTO
        {
            Id = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Title = "Updated Album",
            Status = Status.Private,
            AlbumImg = null
        };
        var category = new Category { Id = editAlbumDTO.CategoryId };
        var existingAlbum = new Album { Id = editAlbumDTO.Id, AlbumImgUrl = "/images/old.jpg" };

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == editAlbumDTO.CategoryId, default))
            .ReturnsAsync(category);
        _unitOfWorkMock.Setup(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == editAlbumDTO.Id, default))
            .ReturnsAsync(existingAlbum);
        _unitOfWorkMock.Setup(uow => uow.Albums.Update(It.IsAny<Album>()));
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _albumService.EditAlbumAsync(editAlbumDTO);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == editAlbumDTO.CategoryId, default), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == editAlbumDTO.Id, default), Times.Once);
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _fileServiceMock.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Albums.Update(It.Is<Album>(a =>
            a.Id == editAlbumDTO.Id &&
            a.Title == editAlbumDTO.Title &&
            a.Status == editAlbumDTO.Status &&
            a.CategoryId == editAlbumDTO.CategoryId &&
            a.AlbumImgUrl == existingAlbum.AlbumImgUrl)), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task EditAlbumAsync_InvalidCategoryId_ThrowsNotFoundException()
    {
        // Arrange
        var editAlbumDTO = new EditAlbumDTO { CategoryId = Guid.NewGuid(), Id = Guid.NewGuid() };

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == editAlbumDTO.CategoryId, default))
            .ReturnsAsync((Category)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _albumService.EditAlbumAsync(editAlbumDTO));
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == editAlbumDTO.CategoryId, default), Times.Once);
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _fileServiceMock.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Albums.Update(It.IsAny<Album>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task EditAlbumAsync_InvalidAlbumId_ThrowsNotFoundException()
    {
        // Arrange
        var editAlbumDTO = new EditAlbumDTO { CategoryId = Guid.NewGuid(), Id = Guid.NewGuid() };
        var category = new Category { Id = editAlbumDTO.CategoryId };

        _unitOfWorkMock.Setup(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == editAlbumDTO.CategoryId, default))
            .ReturnsAsync(category);
        _unitOfWorkMock.Setup(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == editAlbumDTO.Id, default))
            .ReturnsAsync((Album)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _albumService.EditAlbumAsync(editAlbumDTO));
        _unitOfWorkMock.Verify(uow => uow.Categories.GetFirstOrDefaultAsync(c => c.Id == editAlbumDTO.CategoryId, default), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == editAlbumDTO.Id, default), Times.Once);
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _fileServiceMock.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Albums.Update(It.IsAny<Album>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAlbumAsync_ValidId_ReturnsTrue()
    {
        // Arrange
        var albumId = Guid.NewGuid();
        var album = new Album { Id = albumId, AlbumImgUrl = "/images/test.jpg", Songs = new List<Song> { new Song { Id = Guid.NewGuid() } } };

        _unitOfWorkMock.Setup(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == albumId, It.IsAny<Func<IQueryable<Album>, IIncludableQueryable<Album, object>>>()))
            .ReturnsAsync(album);
        _fileServiceMock.Setup(fs => fs.DeleteFile(album.AlbumImgUrl));
        _songServiceMock.Setup(ss => ss.DeleteSongAsync(album.Songs.First().Id))
            .Returns(Task.FromResult(true));
        _unitOfWorkMock.Setup(uow => uow.FavoriteAlbums.Delete(It.IsAny<IEnumerable<FavoriteAlbums>>()));
        _unitOfWorkMock.Setup(uow => uow.UserAlbums.Delete(It.IsAny<IEnumerable<UserAlbum>>()));
        _unitOfWorkMock.Setup(uow => uow.Albums.Delete(album));
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _albumService.DeleteAlbumAsync(albumId);

        // Assert
        Assert.True(result);
        _fileServiceMock.Verify(fs => fs.DeleteFile(album.AlbumImgUrl), Times.Once);
        _songServiceMock.Verify(ss => ss.DeleteSongAsync(album.Songs.First().Id), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.UserAlbums.Delete(It.IsAny<IEnumerable<UserAlbum>>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Albums.Delete(album), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAlbumAsync_InvalidId_ThrowsNotFoundException()
    {
        // Arrange
        var albumId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == albumId, It.IsAny<Func<IQueryable<Album>, IIncludableQueryable<Album, object>>>()))
            .ReturnsAsync((Album)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _albumService.DeleteAlbumAsync(albumId));
        _unitOfWorkMock.Verify(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == albumId, It.IsAny<Func<IQueryable<Album>, IIncludableQueryable<Album, object>>>()), Times.Once);
        _fileServiceMock.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Never);
        _songServiceMock.Verify(ss => ss.DeleteSongAsync(It.IsAny<Guid>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.FavoriteAlbums.Delete(It.IsAny<IEnumerable<FavoriteAlbums>>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.UserAlbums.Delete(It.IsAny<IEnumerable<UserAlbum>>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Albums.Delete(It.IsAny<Album>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task RemoveAlbumFromUserAsync_ValidIds_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var albumId = Guid.NewGuid();
        var userAlbum = new UserAlbum { UserId = userId, AlbumId = albumId };

        _unitOfWorkMock.Setup(uow => uow.UserAlbums.GetFirstOrDefaultAsync(x => x.UserId == userId && x.AlbumId == albumId, default))
            .ReturnsAsync(userAlbum);
        _unitOfWorkMock.Setup(uow => uow.UserAlbums.Delete(userAlbum));
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _albumService.RemoveAlbumFromUserAsync(userId, albumId);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(uow => uow.UserAlbums.GetFirstOrDefaultAsync(x => x.UserId == userId && x.AlbumId == albumId, default), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.UserAlbums.Delete(userAlbum), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoveAlbumFromUserAsync_InvalidIds_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var albumId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.UserAlbums.GetFirstOrDefaultAsync(x => x.UserId == userId && x.AlbumId == albumId, default))
            .ReturnsAsync((UserAlbum)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _albumService.RemoveAlbumFromUserAsync(userId, albumId));
        _unitOfWorkMock.Verify(uow => uow.UserAlbums.GetFirstOrDefaultAsync(x => x.UserId == userId && x.AlbumId == albumId, default), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.UserAlbums.Delete(It.IsAny<UserAlbum>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task AddAlbumToUserAsync_ValidIds_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var albumId = Guid.NewGuid();
        var newUserAlbum = new UserAlbum { UserId = userId, AlbumId = albumId, AddedTime = DateTimeOffset.Now };

        _unitOfWorkMock.Setup(uow => uow.UserAlbums.InsertAsync(It.IsAny<UserAlbum>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.SaveAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _albumService.AddAlbumToUserAsync(userId, albumId);

        // Assert
        Assert.True(result);
        _unitOfWorkMock.Verify(uow => uow.UserAlbums.InsertAsync(It.Is<UserAlbum>(ua => ua.UserId == userId && ua.AlbumId == albumId)), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task AddSongToAlbum_ValidIds_CallsSongService()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var uploadSongDTO = new UploadSongDTO { AlbumId = Guid.NewGuid() };
        var album = new Album { Id = uploadSongDTO?.AlbumId ?? Guid.NewGuid() };

        _unitOfWorkMock.Setup(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == uploadSongDTO.AlbumId, default))
            .ReturnsAsync(album);
        _songServiceMock.Setup(ss => ss.UploadSongAsync(currentUserId, uploadSongDTO))
            .Returns(Task.CompletedTask);

        // Act
        await _albumService.AddSongToAlbum(currentUserId, uploadSongDTO);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == uploadSongDTO.AlbumId, default), Times.Once);
        _songServiceMock.Verify(ss => ss.UploadSongAsync(currentUserId, uploadSongDTO), Times.Once);
    }

    [Fact]
    public async Task AddSongToAlbum_InvalidAlbumId_ThrowsNotFoundException()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var uploadSongDTO = new UploadSongDTO { AlbumId = Guid.NewGuid() };

        _unitOfWorkMock.Setup(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == uploadSongDTO.AlbumId, default))
            .ReturnsAsync((Album)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _albumService.AddSongToAlbum(currentUserId, uploadSongDTO));
        _unitOfWorkMock.Verify(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == uploadSongDTO.AlbumId, default), Times.Once);
        _songServiceMock.Verify(ss => ss.UploadSongAsync(It.IsAny<Guid>(), It.IsAny<UploadSongDTO>()), Times.Never);
    }

    [Fact]
    public async Task RemoveSongFromAlbum_ValidIds_CallsSongService()
    {
        // Arrange
        var albumId = Guid.NewGuid();
        var songId = Guid.NewGuid();
        var album = new Album { Id = albumId };

        _unitOfWorkMock.Setup(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == albumId, default))
            .ReturnsAsync(album);
        _songServiceMock.Setup(ss => ss.DeleteSongAsync(songId))
            .Returns(Task.FromResult(true));

        // Act
        await _albumService.RemoveSongFromAlbum(albumId, songId);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == albumId, default), Times.Once);
        _songServiceMock.Verify(ss => ss.DeleteSongAsync(songId), Times.Once);
    }

    [Fact]
    public async Task RemoveSongFromAlbum_InvalidAlbumId_ThrowsNotFoundException()
    {
        // Arrange
        var albumId = Guid.NewGuid();
        var songId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == albumId, default))
            .ReturnsAsync((Album)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _albumService.RemoveSongFromAlbum(albumId, songId));
        _unitOfWorkMock.Verify(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == albumId, default), Times.Once);
        _songServiceMock.Verify(ss => ss.DeleteSongAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetAlbum_ValidId_ReturnsAlbumDTO()
    {
        // Arrange
        var albumId = Guid.NewGuid();
        var album = new Album { Id = albumId, Category = new Category(), User = new User(), Songs = new List<Song> { new Song { Singer = new User(), Category = new Category() } } };
        var albumDTO = new AlbumDTO();

        _unitOfWorkMock.Setup(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == albumId, It.IsAny<Func<IQueryable<Album>, IIncludableQueryable<Album, object>>>()))
            .ReturnsAsync(album);
        _mapperMock.Setup(m => m.Map<AlbumDTO>(album))
            .Returns(albumDTO);

        // Act
        var result = await _albumService.GetAlbum(albumId);

        // Assert
        Assert.Equal(albumDTO, result);
        _unitOfWorkMock.Verify(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == albumId, It.IsAny<Func<IQueryable<Album>, IIncludableQueryable<Album, object>>>()), Times.Once);
        _mapperMock.Verify(m => m.Map<AlbumDTO>(album), Times.Once);
    }

    [Fact]
    public async Task GetAlbum_InvalidId_ReturnsNull()
    {
        // Arrange
        var albumId = Guid.NewGuid();

        _unitOfWorkMock.Setup(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == albumId, It.IsAny<Func<IQueryable<Album>, IIncludableQueryable<Album, object>>>()))
            .ReturnsAsync((Album)null);
        _mapperMock.Setup(m => m.Map<AlbumDTO>(null))
            .Returns((AlbumDTO)null);

        // Act
        var result = await _albumService.GetAlbum(albumId);

        // Assert
        Assert.Null(result);
        _unitOfWorkMock.Verify(uow => uow.Albums.GetFirstOrDefaultAsync(a => a.Id == albumId, It.IsAny<Func<IQueryable<Album>, IIncludableQueryable<Album, object>>>()), Times.Once);
        _mapperMock.Verify(m => m.Map<AlbumDTO>(null), Times.Once);
    }

    [Fact]
    public async Task GetAlbums_ReturnsListOfAlbumDTOs()
    {
        // Arrange
        var albums = new List<Album> { new Album { Status = Status.Public, Category = new Category(), User = new User(), Songs = new List<Song> { new Song { Singer = new User() } } } };
        var albumDTOs = new List<AlbumDTO> { new AlbumDTO() };

        _unitOfWorkMock.Setup(uow => uow.Albums.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Album, bool>>>(),
            null,
             It.IsAny<Func<IQueryable<Album>, IIncludableQueryable<Album, object>>>(), 0, int.MaxValue))
            .ReturnsAsync(albums);
        _mapperMock.Setup(m => m.Map<IEnumerable<AlbumDTO>>(albums))
            .Returns(albumDTOs);

        // Act
        var result = await _albumService.GetAlbums();

        // Assert
        Assert.Equal(albumDTOs, result);
        _unitOfWorkMock.Verify(uow => uow.Albums.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Album, bool>>>(),
            null,
             It.IsAny<Func<IQueryable<Album>, IIncludableQueryable<Album, object>>>(), 0, int.MaxValue), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<AlbumDTO>>(albums), Times.Once);
    }

    [Fact]
    public async Task GetSingerAlbums_ValidSingerId_ReturnsListOfAlbumDTOs()
    {
        // Arrange
        var singerId = Guid.NewGuid();
        var albums = new List<Album> { new Album { SingerId = singerId, Category = new Category(), User = new User(), Songs = new List<Song> { new Song { Singer = new User() } } } };
        var albumDTOs = new List<AlbumDTO> { new AlbumDTO() };

        _unitOfWorkMock.Setup(uow => uow.Albums.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Album, bool>>>(),
            It.IsAny<Func<IQueryable<Album>, IOrderedQueryable<Album>>>(),
            It.IsAny<Func<IQueryable<Album>, IIncludableQueryable<Album, object>>>(), 0, int.MaxValue))
            .ReturnsAsync(albums);
        _mapperMock.Setup(m => m.Map<IEnumerable<AlbumDTO>>(albums))
            .Returns(albumDTOs);

        // Act
        var result = await _albumService.GetSingerAlbums(singerId);

        // Assert
        Assert.Equal(albumDTOs, result);
        _unitOfWorkMock.Verify(uow => uow.Albums.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Album, bool>>>(),
            It.IsAny<Func<IQueryable<Album>, IOrderedQueryable<Album>>>(),
            It.IsAny<Func<IQueryable<Album>, IIncludableQueryable<Album, object>>>(), 0, int.MaxValue), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<AlbumDTO>>(albums), Times.Once);
    }

    [Fact]
    public async Task GetRecommendsAlbum_ReturnsTop100AlbumDTOsByRate()
    {
        // Arrange
        var albums = Enumerable.Range(0, 150)
            .Select(i => new Album { Status = Status.Public, CountRate = 150 - i, Category = new Category(), User = new User(), Songs = new List<Song> { new Song { Singer = new User() } } })
            .ToList();
        var top100Albums = albums.Take(100).ToList();
        var albumDTOs = top100Albums.Select(a => new AlbumDTO()).ToList();

        _unitOfWorkMock.Setup(uow => uow.Albums.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Album, bool>>>(),
            It.IsAny<Func<IQueryable<Album>, IOrderedQueryable<Album>>>(),
             It.IsAny<Func<IQueryable<Album>, IIncludableQueryable<Album, object>>>(), 0, int.MaxValue))
            .ReturnsAsync(albums);
        _mapperMock.Setup(m => m.Map<IEnumerable<AlbumDTO>>(top100Albums))
            .Returns(albumDTOs);

        // Act
        var result = await _albumService.GetRecommendsAlbum();

        // Assert
        Assert.Equal(albumDTOs, result);
        _unitOfWorkMock.Verify(uow => uow.Albums.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Album, bool>>>(),
            It.IsAny<Func<IQueryable<Album>, IOrderedQueryable<Album>>>(),
            It.IsAny<Func<IQueryable<Album>, IIncludableQueryable<Album, object>>>(), 0, int.MaxValue), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<AlbumDTO>>(top100Albums), Times.Once);
    }
}
