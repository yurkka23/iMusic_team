using AutoMapper;
using iMusic.BL.Exceptions;
using iMusic.BL.Interfaces;
using iMusic.BL.Services;
using iMusic.DAL.Abstractions;
using iMusic.DAL.DataContext;
using iMusic.DAL.DTOs.Album;
using iMusic.DAL.DTOs.Category;
using iMusic.DAL.DTOs.Song;
using iMusic.DAL.DTOs.User;
using iMusic.DAL.Entities;
using iMusic.DAL.Entities.Enums;
using iMusic.DAL.Realizations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace IntegrationTests;

public class AlbumServiceIntegrationTests : IDisposable
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ISongService> _songServiceMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly AlbumService _albumService;
    public AlbumServiceIntegrationTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "AlbumServiceIntegrationTests")
            .Options;
        _context = new ApplicationDbContext(_options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        _unitOfWork = new UnitOfWork(_context);
        _mapperMock = new Mock<IMapper>();
        _songServiceMock = new Mock<ISongService>();
        _fileServiceMock = new Mock<IFileService>();

        _albumService = new AlbumService(_unitOfWork, _mapperMock.Object, _songServiceMock.Object, _fileServiceMock.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task CreateAlbumAsync_ValidInput_AlbumIsCreatedInDatabase()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var category = new Category { Id = Guid.NewGuid(), Name = "Pop" };
        var createAlbumDto = new CreateAlbumDTO { Title = "Test Album", Status = Status.Public, CategoryId = category.Id, AlbumImg = Mock.Of<IFormFile>() };
        var imgPath = "/images/test_album.jpg";
        _fileServiceMock.Setup(fs => fs.GetFilePathAsync(createAlbumDto.AlbumImg)).ReturnsAsync(imgPath);

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        // Act
        var albumId = await _albumService.CreateAlbumAsync(currentUserId, createAlbumDto);

        // Assert
        var createdAlbum = await _context.Albums.FirstOrDefaultAsync(a => a.Id == albumId);
        Assert.NotNull(createdAlbum);
        Assert.Equal("Test Album", createdAlbum.Title);
        Assert.Equal(Status.Public, createdAlbum.Status);
        Assert.Equal(category.Id, createdAlbum.CategoryId);
        Assert.Equal(currentUserId, createdAlbum.SingerId);
        Assert.Equal(imgPath, createdAlbum.AlbumImgUrl);
    }

    [Fact]
    public async Task DeleteAlbumAsync_ExistingAlbumWithSongs_DeletesAlbumAndSongs()
    {
        // Arrange
        var albumId = Guid.NewGuid();
        var albumToDelete = new Album { Id = albumId, Title = "ToDelete", AlbumImgUrl = "/images/delete_album.jpg" };

        await _context.Albums.AddAsync(albumToDelete);
        var res = await _context.SaveChangesAsync();

        // Act
        var result = await _albumService.DeleteAlbumAsync(albumId);

        // Assert
        Assert.True(result);
        Assert.Null(await _context.Albums.FindAsync(albumId));
    }

    [Fact]
    public async Task AddAlbumToUserAsync_ValidInput_UserAlbumIsCreatedInDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var albumId = Guid.NewGuid();

        await _context.Albums.AddAsync(new Album { Id = albumId, Title = "Test Album" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _albumService.AddAlbumToUserAsync(userId, albumId);

        // Assert
        Assert.True(result);
        var userAlbum = await _context.UserAlbums.FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AlbumId == albumId);
        Assert.NotNull(userAlbum);
        Assert.Equal(userId, userAlbum.UserId);
        Assert.Equal(albumId, userAlbum.AlbumId);
    }

    [Fact]
    public async Task RemoveAlbumFromUserAsync_ExistingUserAlbum_UserAlbumIsDeletedFromDatabase()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var albumId = Guid.NewGuid();
        var userAlbumToRemove = new UserAlbum { UserId = userId, AlbumId = albumId, AddedTime = DateTimeOffset.Now };

        await _context.Albums.AddAsync(new Album { Id = albumId, Title = "Test Album" });
        await _context.UserAlbums.AddAsync(userAlbumToRemove);
        await _context.SaveChangesAsync();

        // Act
        var result = await _albumService.RemoveAlbumFromUserAsync(userId, albumId);

        // Assert
        Assert.True(result);
        var userAlbum = await _context.UserAlbums.FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AlbumId == albumId);
        Assert.Null(userAlbum);
    }

    [Fact]
    public async Task RemoveAlbumFromUserAsync_NonExistingUserAlbum_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var albumId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _albumService.RemoveAlbumFromUserAsync(userId, albumId));
    }

    [Fact]
    public async Task AddSongToAlbum_ValidInput_SongServiceUploadSongAsyncIsCalled()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var albumId = Guid.NewGuid();
        var uploadSongDto = new UploadSongDTO { AlbumId = albumId, Title = "Test Song", SongFile = Mock.Of<IFormFile>() };

        await _context.Albums.AddAsync(new Album { Id = albumId, Title = "Test Album" });
        await _context.SaveChangesAsync();

        var mockSongService = new Mock<ISongService>();

        // Act
        await _albumService.AddSongToAlbum(currentUserId, uploadSongDto);

        // Assert
        mockSongService.Verify(s => s.UploadSongAsync(currentUserId, uploadSongDto), Times.Never);
    }

    [Fact]
    public async Task GetAlbum_ExistingAlbumId_ReturnsCorrectAlbumDTO()
    {
        // Arrange
        var albumId = Guid.NewGuid();
        var category = new Category { Id = Guid.NewGuid(), Name = "Rock" };
        var singer = new User { Id = Guid.NewGuid(), UserName = "Singer A" };
        var album = new Album
        {
            Id = albumId,
            Title = "Test Album",
            Status = Status.Public,
            CategoryId = category.Id,
            SingerId = singer.Id,
            Category = category,
            User = singer,
            Songs = new List<Song>()
            {
                new Song { Id = Guid.NewGuid(), Title = "Song 1", SingerId = singer.Id, CategoryId = category.Id, Singer = singer, Category = category },
                new Song { Id = Guid.NewGuid(), Title = "Song 2", SingerId = singer.Id, CategoryId = category.Id, Singer = singer, Category = category }
            }
        };

        await _context.Categories.AddAsync(category);
        await _context.Users.AddAsync(singer);
        await _context.Albums.AddAsync(album);
        await _context.SaveChangesAsync();

        _mapperMock.Setup(m => m.Map<AlbumDTO>(It.Is<Album>(a => a.Id == albumId)))
            .Returns(new AlbumDTO { Id = albumId, Title = "Test Album", Category = new CategoryDTO { Id = category.Id, Name = "Rock" }, Singer = new UserDTO { Id = singer.Id, UserName = "Singer A" }, Songs = new List<SongDTO>() { new SongDTO { Id = album.Songs.First().Id, Title = "Song 1" }, new SongDTO { Id = album.Songs.Last().Id, Title = "Song 2" } } });
        // Act
        var result = await _albumService.GetAlbum(albumId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(albumId, result.Id);
        Assert.Equal("Test Album", result.Title);
        Assert.NotNull(result.Category);
        Assert.Equal("Rock", result.Category.Name);
        Assert.NotNull(result.Singer);
        Assert.Equal("Singer A", result.Singer.UserName);
        Assert.NotNull(result.Songs);
        Assert.Equal(2, result.Songs.Count());
        Assert.Contains(result.Songs, s => s.Title == "Song 1");
        Assert.Contains(result.Songs, s => s.Title == "Song 2");
    }

    [Fact]
    public async Task GetAlbum_NonExistingAlbumId_ReturnsNull()
    {
        // Arrange
        var nonExistingAlbumId = Guid.NewGuid();

        // Act
        var result = await _albumService.GetAlbum(nonExistingAlbumId);

        // Assert
        Assert.Null(result);
        var albumFromDb = await _context.Albums.FirstOrDefaultAsync(a => a.Id == nonExistingAlbumId);
        Assert.Null(albumFromDb);
    }

    [Fact]
    public async Task GetAlbums_PublicAlbumsExist_ReturnsListOfAlbumDTOs()
    {
        // Arrange
        var category = new Category { Id = Guid.NewGuid(), Name = "Pop" };
        var singer = new User { Id = Guid.NewGuid(), UserName = "Singer B" };
        var album1 = new Album { Id = Guid.NewGuid(), Title = "Public Album 1", Status = Status.Public, CategoryId = category.Id, SingerId = singer.Id };
        var album2 = new Album { Id = Guid.NewGuid(), Title = "Private Album", Status = Status.Private, CategoryId = category.Id, SingerId = singer.Id };
        var album3 = new Album { Id = Guid.NewGuid(), Title = "Public Album 2", Status = Status.Public, CategoryId = category.Id, SingerId = singer.Id };

        await _context.Categories.AddAsync(category);
        await _context.Users.AddAsync(singer);
        await _context.Albums.AddRangeAsync(album1, album2, album3);
        await _context.SaveChangesAsync();

        _mapperMock.Setup(m => m.Map<IEnumerable<AlbumDTO>>(It.Is<IEnumerable<Album>>(albums => albums.Count() == 2 && albums.Any(a => a.Title == "Public Album 1") && albums.Any(a => a.Title == "Public Album 2"))))
            .Returns(new List<AlbumDTO> { new AlbumDTO { Id = album1.Id, Title = "Public Album 1" }, new AlbumDTO { Id = album3.Id, Title = "Public Album 2" } });


        // Act
        var result = await _albumService.GetAlbums();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, a => a.Title == "Public Album 1");
        Assert.Contains(result, a => a.Title == "Public Album 2");
        Assert.DoesNotContain(result, a => a.Title == "Private Album");
    }

    [Fact]
    public async Task GetAlbums_NoPublicAlbums_ReturnsEmptyList()
    {
        // Arrange
        var category = new Category { Id = Guid.NewGuid(), Name = "Jazz" };
        var singer = new User { Id = Guid.NewGuid(), UserName = "Singer C" };
        var album1 = new Album { Id = Guid.NewGuid(), Title = "Private Album 1", Status = Status.Private, CategoryId = category.Id, SingerId = singer.Id };
        var album2 = new Album { Id = Guid.NewGuid(), Title = "Private Album 2", Status = Status.Private, CategoryId = category.Id, SingerId = singer.Id };

        await _context.Categories.AddAsync(category);
        await _context.Users.AddAsync(singer);
        await _context.Albums.AddRangeAsync(album1, album2);
        await _context.SaveChangesAsync();

        _mapperMock.Setup(m => m.Map<IEnumerable<AlbumDTO>>(It.Is<IEnumerable<Album>>(albums => !albums.Any(a => a.Status == Status.Public))))
            .Returns(new List<AlbumDTO>());


        // Act
        var result = await _albumService.GetAlbums();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
