using AutoMapper;
using iMusic.BL.Exceptions;
using iMusic.BL.Interfaces;
using iMusic.BL.Services;
using iMusic.DAL.Abstractions;
using iMusic.DAL.Constants;
using iMusic.DAL.DTOs.Search;
using iMusic.DAL.DTOs.User;
using iMusic.DAL.Entities;
using iMusic.DAL.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Moq;

namespace UnitTests;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<IHttpContextAccessor> _contextAccessorMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IOptionsSnapshot<HostSettings>> _hostSettingsMock;
    private readonly Mock<RoleManager<AppRole>> _roleManagerMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _fileServiceMock = new Mock<IFileService>();
        _contextAccessorMock = new Mock<IHttpContextAccessor>();
        _userManagerMock = MockHelpers.MockUserManager<User>();
        _hostSettingsMock = new Mock<IOptionsSnapshot<HostSettings>>();
        _roleManagerMock = MockHelpers.MockRoleManager<AppRole>();

        _hostSettingsMock.Setup(s => s.Value).Returns(new HostSettings { CurrentHost = "http://localhost" });

        _userService = new UserService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _fileServiceMock.Object,
            _contextAccessorMock.Object,
            _userManagerMock.Object,
            _hostSettingsMock.Object,
            _roleManagerMock.Object);
    }

    [Fact]
    public async Task GetCurrentUserInformationAsync_ExistingUser_ReturnsUserDTOWithRoles()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var user = new User { Id = currentUserId, UserName = "testuser", Email = "test@example.com" };
        var userRoles = new List<string> { "Role1", "Role2" };
        var userDTO = new UserDTO { Id = currentUserId, UserName = "testuser", Email = "test@example.com", UserRoles = new List<string>() };

        _unitOfWorkMock.Setup(uow => uow.Users.GetFirstOrDefaultAsync(u => u.Id == currentUserId, default))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.GetRolesAsync(user))
            .ReturnsAsync(userRoles);
        _mapperMock.Setup(m => m.Map<UserDTO>(user))
            .Returns(userDTO);

        // Act
        var result = await _userService.GetCurrentUserInformationAsync(currentUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(currentUserId, result.Id);
        Assert.Equal(userRoles.Count, result.UserRoles.Count);
        Assert.Contains("Role1", result.UserRoles);
        Assert.Contains("Role2", result.UserRoles);
        _unitOfWorkMock.Verify(uow => uow.Users.GetFirstOrDefaultAsync(u => u.Id == currentUserId, default), Times.Once);
        _userManagerMock.Verify(um => um.GetRolesAsync(user), Times.Once);
        _mapperMock.Verify(m => m.Map<UserDTO>(user), Times.Once);
    }

    [Fact]
    public async Task GetCurrentUserInformationAsync_NonExistingUser_ThrowsNotFoundException()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        _unitOfWorkMock.Setup(uow => uow.Users.GetFirstOrDefaultAsync(u => u.Id == currentUserId, default))
            .ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetCurrentUserInformationAsync(currentUserId));
        _unitOfWorkMock.Verify(uow => uow.Users.GetFirstOrDefaultAsync(u => u.Id == currentUserId, default), Times.Once);
        _userManagerMock.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Never);
        _mapperMock.Verify(m => m.Map<UserDTO>(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task GetUserByIdAsync_ExistingUserNotBanned_ReturnsUserDTOWithRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, UserName = "testuser", Email = "test@example.com", IsBanned = false };
        var userRoles = new List<string> { "RoleA", "RoleB" };
        var userDTO = new UserDTO { Id = userId, UserName = "testuser", Email = "test@example.com", UserRoles = new List<string>() };

        _unitOfWorkMock.Setup(uow => uow.Users.GetFirstOrDefaultAsync(u => u.Id == userId, default))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.GetRolesAsync(user))
            .ReturnsAsync(userRoles);
        _mapperMock.Setup(m => m.Map<UserDTO>(user))
            .Returns(userDTO);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal(userRoles.Count, result.UserRoles.Count);
        Assert.Contains("RoleA", result.UserRoles);
        Assert.Contains("RoleB", result.UserRoles);
        _unitOfWorkMock.Verify(uow => uow.Users.GetFirstOrDefaultAsync(u => u.Id == userId, default), Times.Once);
        _userManagerMock.Verify(um => um.GetRolesAsync(user), Times.Once);
        _mapperMock.Verify(m => m.Map<UserDTO>(user), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_NonExistingUser_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _unitOfWorkMock.Setup(uow => uow.Users.GetFirstOrDefaultAsync(u => u.Id == userId, default))
            .ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUserByIdAsync(userId));
        _unitOfWorkMock.Verify(uow => uow.Users.GetFirstOrDefaultAsync(u => u.Id == userId, default), Times.Once);
        _userManagerMock.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Never);
        _mapperMock.Verify(m => m.Map<UserDTO>(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task GetUserByIdAsync_BannedUser_ThrowsBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, IsBanned = true };
        _unitOfWorkMock.Setup(uow => uow.Users.GetFirstOrDefaultAsync(u => u.Id == userId, default))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _userService.GetUserByIdAsync(userId));
        _unitOfWorkMock.Verify(uow => uow.Users.GetFirstOrDefaultAsync(u => u.Id == userId, default), Times.Once);
        _userManagerMock.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Never);
        _mapperMock.Verify(m => m.Map<UserDTO>(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task SearchUsersAsync_MatchingUsers_ReturnsUserDTOs()
    {
        // Arrange
        var searchTerm = "test";
        var users = new List<User>
            {
                new User { UserName = "testuser1", Email = "test1@example.com", FirstName = "Test", LastName = "One", IsBanned = false, UserRoles = new List<AppUserRole>() },
                new User { UserName = "user2test", Email = "two@test.com", FirstName = "Second", LastName = "User", IsBanned = false, UserRoles = new List<AppUserRole>() },
                new User { UserName = "banneduser", Email = "banned@example.com", FirstName = "Banned", LastName = "User", IsBanned = true, UserRoles = new List<AppUserRole>() }
            };
        var expectedUsers = users.Where(u => (u.UserName.Contains(searchTerm) || u.Email.Contains(searchTerm) || (u.FirstName + ' ' + u.LastName).Contains(searchTerm)) && !u.IsBanned).ToList();
        var expectedDTOs = expectedUsers.Select(u => new UserDTO { UserName = u.UserName, Email = u.Email }).ToList();

        _unitOfWorkMock.Setup(uow => uow.Users.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            null,
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(), 0, int.MaxValue))
            .ReturnsAsync(users);
        _mapperMock.Setup(m => m.Map<IEnumerable<UserDTO>>(expectedUsers))
            .Returns(expectedDTOs);

        // Act
        var result = await _userService.SearchUsersAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.All(result, dto => expectedDTOs.Any(e => e.UserName == dto.UserName && e.Email == dto.Email));
        _unitOfWorkMock.Verify(uow => uow.Users.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            null,
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(), 0, int.MaxValue), Times.Once);
    }

    [Fact]
    public async Task SearchUsersAsync_NoMatchingUsers_ReturnsEmptyList()
    {
        // Arrange
        var searchTerm = "nomatch";
        var users = new List<User>
            {
                new User { UserName = "testuser1", Email = "test1@example.com", FirstName = "Test", LastName = "One", IsBanned = false, UserRoles = new List<AppUserRole>() }
            };
        _unitOfWorkMock.Setup(uow => uow.Users.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            null,
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(), 0, int.MaxValue))
            .ReturnsAsync(users);
        _mapperMock.Setup(m => m.Map<IEnumerable<UserDTO>>(It.IsAny<List<User>>()))
            .Returns(new List<UserDTO>());

        // Act
        var result = await _userService.SearchUsersAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _unitOfWorkMock.Verify(uow => uow.Users.GetAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            null,
            It.IsAny<Func<IQueryable<User>, IIncludableQueryable<User, object>>>(), 0, int.MaxValue), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<UserDTO>>(It.IsAny<List<User>>()), Times.Once);
    }

    [Fact]
    public async Task SearchSingersAsync_MatchingSingers_ReturnsUserDTOs()
    {
        // Arrange
        var searchDTO = new SearchDTO { SearchTerm = "singer" };
        var singers = new List<User>
            {
                new User { UserName = "singer1", FirstName = "Singer", LastName = "One", IsBanned = false },
                new User { UserName = "user2", FirstName = "Regular", LastName = "User", IsBanned = false },
                new User { UserName = "singerbanned", FirstName = "Banned", LastName = "Singer", IsBanned = true }
            };
        var expectedSingers = singers.Where(s => (s.UserName.Contains(searchDTO.SearchTerm) || (s.FirstName + ' ' + s.LastName).Contains(searchDTO.SearchTerm)) && !s.IsBanned).ToList();
        var expectedDTOs = expectedSingers.Select(s => new UserDTO { UserName = s.UserName, FirstName = s.FirstName, LastName = s.LastName }).ToList();

        _userManagerMock.Setup(um => um.GetUsersInRoleAsync(RoleConstants.SingerRole))
            .ReturnsAsync(singers);
        _mapperMock.Setup(m => m.Map<IEnumerable<UserDTO>>(expectedSingers))
            .Returns(expectedDTOs);

        // Act
        var result = await _userService.SearchSingersAsync(searchDTO);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDTOs.Count, result.Count());
        Assert.All(result, dto => expectedDTOs.Any(e => e.UserName == dto.UserName && e.FirstName == dto.FirstName && e.LastName == dto.LastName));
        _userManagerMock.Verify(um => um.GetUsersInRoleAsync(RoleConstants.SingerRole), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<UserDTO>>(expectedSingers), Times.Once);
    }

    [Fact]
    public async Task SearchSingersAsync_NoMatchingSingers_ReturnsEmptyList()
    {
        // Arrange
        var searchDTO = new SearchDTO { SearchTerm = "nomatch" };
        var singers = new List<User>
            {
                new User { UserName = "singer1", FirstName = "Singer", LastName = "One", IsBanned = false }
            };

        _userManagerMock.Setup(um => um.GetUsersInRoleAsync(RoleConstants.SingerRole))
            .ReturnsAsync(singers);
        _mapperMock.Setup(m => m.Map<IEnumerable<UserDTO>>(It.IsAny<List<User>>()))
            .Returns(new List<UserDTO>());

        // Act
        var result = await _userService.SearchSingersAsync(searchDTO);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _userManagerMock.Verify(um => um.GetUsersInRoleAsync(RoleConstants.SingerRole), Times.Once);
    }
}
