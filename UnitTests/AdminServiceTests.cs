using AutoMapper;
using iMusic.BL.Services;
using iMusic.DAL.Abstractions;
using iMusic.DAL.DTOs.Album;
using iMusic.DAL.DTOs.Song;
using iMusic.DAL.DTOs.User;
using iMusic.DAL.Entities;
using iMusic.DAL.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Moq;

namespace UnitTests;

public class AdminServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<RoleManager<AppRole>> _roleManagerMock;
    private readonly Mock<IOptionsSnapshot<HostSettings>> _hostSettingsMock;
    private readonly AdminService _adminService;
    private readonly HostSettings _hostSettings;

    public AdminServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _userManagerMock = MockHelpers.MockUserManager<User>();
        _roleManagerMock = MockHelpers.MockRoleManager<AppRole>();
        _hostSettingsMock = new Mock<IOptionsSnapshot<HostSettings>>();
        _hostSettings = new HostSettings { CurrentHost = "http://localhost" };
        _hostSettingsMock.Setup(s => s.Value).Returns(_hostSettings);
        _adminService = new AdminService(_unitOfWorkMock.Object, _mapperMock.Object, _userManagerMock.Object, _hostSettingsMock.Object, _roleManagerMock.Object);
    }

    [Fact]
    public async Task GetUserInfoAsync_ValidId_ReturnsUserDTO()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, UserName = "testuser" };
        var userDTO = new UserDTO { Id = userId, UserName = "testuser" };

        _unitOfWorkMock.Setup(uow => uow.Users.GetFirstOrDefaultAsync(u => u.Id == userId, default))
            .ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDTO>(user))
            .Returns(userDTO);

        // Act
        var result = await _adminService.GetUserInfoAsync(userId);

        // Assert
        Assert.Equal(userDTO, result);
        _unitOfWorkMock.Verify(uow => uow.Users.GetFirstOrDefaultAsync(u => u.Id == userId, default), Times.Once);
        _mapperMock.Verify(m => m.Map<UserDTO>(user), Times.Once);
    }

    [Fact]
    public async Task GetUserAddedSongsAsync_ValidId_ReturnsListOfSongDTOs()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var songs = new List<Song> { new Song { Id = Guid.NewGuid(), Title = "Song 1" }, new Song { Id = Guid.NewGuid(), Title = "Song 2" } };
        var userSongs = new List<UserSong>
            {
                new UserSong { UserId = userId, Song = songs[0], AddedTime = DateTimeOffset.Now },
                new UserSong { UserId = userId, Song = songs[1], AddedTime = DateTimeOffset.Now.AddDays(-1) }
            };
        var songDTOs = new List<SongDTO> { new SongDTO { Id = songs[0].Id, Title = songs[0].Title }, new SongDTO { Id = songs[1].Id, Title = songs[1].Title } };

        _unitOfWorkMock.Setup(uow => uow.UserSongs.GetAsync(
            x => x.UserId == userId,
            It.IsAny<Func<IQueryable<UserSong>, IOrderedQueryable<UserSong>>>(),
            It.IsAny<Func<IQueryable<UserSong>, IIncludableQueryable<UserSong, object>>>(), 0, int.MaxValue))
            .ReturnsAsync(userSongs);
        _mapperMock.Setup(m => m.Map<IEnumerable<SongDTO>>(songs.OrderByDescending(s => s.Title))) // Assuming order by AddedTime in query
            .Returns(songDTOs.OrderByDescending(s => s.Title));

        // Act
        var result = await _adminService.GetUserAddedSongsAsync(userId);

        // Assert
        Assert.NotNull(result);
        _unitOfWorkMock.Verify(uow => uow.UserSongs.GetAsync(
            x => x.UserId == userId,
            It.IsAny<Func<IQueryable<UserSong>, IOrderedQueryable<UserSong>>>(),
            It.IsAny<Func<IQueryable<UserSong>, IIncludableQueryable<UserSong, object>>>(), 0, int.MaxValue), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<SongDTO>>(It.IsAny<IEnumerable<Song>>()), Times.Once);
    }

    [Fact]
    public async Task GetUserAddedAlbumsAsync_ValidId_ReturnsListOfAlbumDTOs()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var albums = new List<Album> { new Album { Id = Guid.NewGuid(), Title = "Album 1" }, new Album { Id = Guid.NewGuid(), Title = "Album 2" } };
        var userAlbums = new List<UserAlbum>
            {
                new UserAlbum { UserId = userId, Album = albums[0], AddedTime = DateTimeOffset.Now },
                new UserAlbum { UserId = userId, Album = albums[1], AddedTime = DateTimeOffset.Now.AddDays(-1) }
            };
        var albumDTOs = new List<AlbumDTO> { new AlbumDTO { Id = albums[0].Id, Title = albums[0].Title }, new AlbumDTO { Id = albums[1].Id, Title = albums[1].Title } };

        _unitOfWorkMock.Setup(uow => uow.UserAlbums.GetAsync(
            x => x.UserId == userId,
            It.IsAny<Func<IQueryable<UserAlbum>, IOrderedQueryable<UserAlbum>>>(),
            It.IsAny<Func<IQueryable<UserAlbum>, IIncludableQueryable<UserAlbum, object>>>(), 0, int.MaxValue))
            .ReturnsAsync(userAlbums);
        _unitOfWorkMock.Setup(uow => uow.Albums.GetAsync(
            x => It.IsAny<List<Guid>>().Contains(x.Id),
            It.IsAny<Func<IQueryable<Album>, IOrderedQueryable<Album>>>(),
            It.IsAny<Func<IQueryable<Album>, IIncludableQueryable<Album, object>>>(), 0, int.MaxValue))
            .ReturnsAsync(albums);
        _mapperMock.Setup(m => m.Map<IEnumerable<AlbumDTO>>(albums.OrderByDescending(a => a.Title))) // Assuming order by AddedTime
            .Returns(albumDTOs.OrderByDescending(a => a.Title));

        // Act
        var result = await _adminService.GetUserAddedAlbumsAsync(userId);

        // Assert
        Assert.NotNull(result);
        _unitOfWorkMock.Verify(uow => uow.UserAlbums.GetAsync(
            x => x.UserId == userId,
            It.IsAny<Func<IQueryable<UserAlbum>, IOrderedQueryable<UserAlbum>>>(),
            It.IsAny<Func<IQueryable<UserAlbum>, IIncludableQueryable<UserAlbum, object>>>(), 0, int.MaxValue), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Albums.GetAsync(
            x => It.IsAny<List<Guid>>().Contains(x.Id),
            It.IsAny<Func<IQueryable<Album>, IOrderedQueryable<Album>>>(),
            It.IsAny<Func<IQueryable<Album>, IIncludableQueryable<Album, object>>>(), 0, int.MaxValue), Times.Never);
        _mapperMock.Verify(m => m.Map<IEnumerable<AlbumDTO>>(It.IsAny<IEnumerable<Album>>()), Times.Once);
    }
}
