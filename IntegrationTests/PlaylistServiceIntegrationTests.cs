using AutoMapper;
using iMusic.BL.Exceptions;
using iMusic.BL.Interfaces;
using iMusic.BL.Services;
using iMusic.DAL.DataContext;
using iMusic.DAL.DTOs.Playlist;
using iMusic.DAL.Entities;
using iMusic.DAL.Entities.Enums;
using iMusic.DAL.Realizations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
namespace IntegrationTests;

public class PlaylistServiceIntegrationTests : IDisposable
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly ApplicationDbContext _context;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ISongService> _songServiceMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly PlayListService _playListService;
    private readonly UnitOfWork _unitOfWork;

    public PlaylistServiceIntegrationTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "PlaylistServiceIntegrationTests")
            .Options;
        _context = new ApplicationDbContext(_options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        _unitOfWork = new UnitOfWork(_context);
        _mapperMock = new Mock<IMapper>();
        _songServiceMock = new Mock<ISongService>();
        _fileServiceMock = new Mock<IFileService>();

        _playListService = new PlayListService(_unitOfWork, _mapperMock.Object, _songServiceMock.Object, _fileServiceMock.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task CreatePlayListAsync_ValidInput_PlaylistIsCreatedInDatabase()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var createPlaylistDto = new CreatePlayListDTO { Title = "Test Playlist", Status = Status.Public, PlayListImg = new Mock<IFormFile>().Object };
        var imgPath = "/images/test.jpg";
        _fileServiceMock.Setup(fs => fs.GetFilePathAsync(createPlaylistDto.PlayListImg)).ReturnsAsync(imgPath);

        // Act
        await _playListService.CreatePlayListAsync(currentUserId, createPlaylistDto);
        await _context.SaveChangesAsync();

        // Assert
        var createdPlaylist = await _context.Playlists.FirstOrDefaultAsync(p => p.Title == "Test Playlist");
        Assert.NotNull(createdPlaylist);
        Assert.Equal(currentUserId, createdPlaylist.AuthorId);
        Assert.Equal(imgPath, createdPlaylist.PlaylistImgUrl);
    }

    [Fact]
    public async Task DeletePlaylistAsync_ExistingPlaylist_RemovesPlaylistAndRelatedEntities()
    {
        // Arrange
        var playlistId = Guid.NewGuid();
        var playlistToDelete = new Playlist { Id = playlistId, Title = "ToDelete", PlaylistImgUrl = "/images/delete.jpg" };

        await _unitOfWork.Playlists.InsertAsync(playlistToDelete);
        await _unitOfWork.SaveAsync();

        _fileServiceMock.Setup(fs => fs.DeleteFile(playlistToDelete.PlaylistImgUrl));

        // Act
        var result = await _playListService.DeletePlaylistAsync(playlistId);
        await _context.SaveChangesAsync();

        // Assert
        Assert.True(result);
        Assert.Null(await _context.Playlists.FindAsync(playlistId));
        Assert.Empty(await _context.FavoritePlaylists.Where(fp => fp.PlaylistId == playlistId).ToListAsync());
        Assert.Empty(await _context.SongPlaylists.Where(sp => sp.PlaylistId == playlistId).ToListAsync());
    }

    [Fact]
    public async Task DeletePlaylistAsync_NonExistingPlaylist_ThrowsNotFoundException()
    {
        // Arrange
        var playlistId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _playListService.DeletePlaylistAsync(playlistId));
        _fileServiceMock.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task EditPlayListAsync_ValidInput_UpdatesPlaylistInDatabase()
    {
        // Arrange
        var playlistId = Guid.NewGuid();
        var existingPlaylist = new Playlist { Id = playlistId, Title = "Old Title", Status = Status.Public, PlaylistImgUrl = "/images/old.jpg" };
        var editPlaylistDto = new EditPlayListDTO { Id = playlistId, Title = "New Title", Status = Status.Private, PlaylistImg = new Mock<IFormFile>().Object };
        var newImagePath = "/images/new.jpg";

        await _context.Playlists.AddAsync(existingPlaylist);
        await _context.SaveChangesAsync();


        _fileServiceMock.Setup(fs => fs.GetFilePathAsync(editPlaylistDto.PlaylistImg)).ReturnsAsync(newImagePath);
        _fileServiceMock.Setup(fs => fs.DeleteFile(existingPlaylist.PlaylistImgUrl));

        // Act
        var result = await _playListService.EditPlayListAsync(editPlaylistDto);
        await _context.SaveChangesAsync();

        // Assert
        Assert.True(result);
        var updatedPlaylist = await _context.Playlists.FindAsync(playlistId);
        Assert.NotNull(updatedPlaylist);
        Assert.Equal("New Title", updatedPlaylist.Title);
        Assert.Equal(Status.Private, updatedPlaylist.Status);
        Assert.Equal(newImagePath, updatedPlaylist.PlaylistImgUrl);
    }

    [Fact]
    public async Task EditPlayListAsync_InvalidId_ThrowsNotFoundException()
    {
        // Arrange
        var editPlaylistDto = new EditPlayListDTO { Id = Guid.NewGuid(), Title = "New Title", Status = Status.Private, PlaylistImg = new Mock<IFormFile>().Object };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _playListService.EditPlayListAsync(editPlaylistDto));
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
        _fileServiceMock.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ChangeImageAsync_ValidInput_UpdatesPlaylistImageInDatabase()
    {
        // Arrange
        var playlistId = Guid.NewGuid();
        var existingPlaylist = new Playlist { Id = playlistId, PlaylistImgUrl = "/images/old.jpg" };
        var changePlaylistImageDto = new ChangePlaylistImageDTO { Id = playlistId, PlayListImg = new Mock<IFormFile>().Object };
        var newImagePath = "/images/new.jpg";

        await _context.Playlists.AddAsync(existingPlaylist);
        await _context.SaveChangesAsync();

        _fileServiceMock.Setup(fs => fs.GetFilePathAsync(changePlaylistImageDto.PlayListImg)).ReturnsAsync(newImagePath);
        _fileServiceMock.Setup(fs => fs.DeleteFile(existingPlaylist.PlaylistImgUrl));

        // Act
        var result = await _playListService.ChangeImageAsync(changePlaylistImageDto);
        await _context.SaveChangesAsync();

        // Assert
        Assert.True(result);
        var updatedPlaylist = await _context.Playlists.FindAsync(playlistId);
        Assert.NotNull(updatedPlaylist);
        Assert.Equal(newImagePath, updatedPlaylist.PlaylistImgUrl);
    }

    [Fact]
    public async Task ChangeImageAsync_InvalidId_ThrowsNotFoundException()
    {
        // Arrange
        var changePlaylistImageDto = new ChangePlaylistImageDTO { Id = Guid.NewGuid(), PlayListImg = new Mock<IFormFile>().Object };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _playListService.ChangeImageAsync(changePlaylistImageDto));
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(It.IsAny<IFormFile>()), Times.Never);
    }

    [Fact]
    public async Task ChangeImageAsync_NoImageFile_ThrowsNoFileException()
    {
        // Arrange
        var playlistId = Guid.NewGuid();
        var existingPlaylist = new Playlist { Id = playlistId, PlaylistImgUrl = "/images/old.jpg" };
        var changePlaylistImageDto = new ChangePlaylistImageDTO { Id = playlistId, PlayListImg = null };

        await _context.Playlists.AddAsync(existingPlaylist);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<NoFileException>(() => _playListService.ChangeImageAsync(changePlaylistImageDto));
        _fileServiceMock.Verify(fs => fs.GetFilePathAsync(null), Times.Never);
        _fileServiceMock.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Never);
    }
}
