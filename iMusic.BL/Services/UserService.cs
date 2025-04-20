using AutoMapper;
using IdentityModel;
using iMusic.BL.Exceptions;
using iMusic.BL.Interfaces;
using iMusic.DAL.Abstractions;
using iMusic.DAL.Constants;
using iMusic.DAL.DTOs.Search;
using iMusic.DAL.DTOs.User;
using iMusic.DAL.Entities;
using iMusic.DAL.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;

namespace iMusic.BL.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly UserManager<User> _userManager;
    private readonly HostSettings _hostSettings;
    private readonly RoleManager<AppRole> _roleManager;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService, 
        IHttpContextAccessor contextAccessor, UserManager<User> userManager, IOptionsSnapshot<HostSettings> hostSettings, RoleManager<AppRole> roleManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileService = fileService;
        _contextAccessor = contextAccessor;
        _userManager = userManager;
        _hostSettings = hostSettings.Value;
        _roleManager = roleManager;
    }

    public async Task<UserDTO> GetCurrentUserInformationAsync(Guid currentUserId)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Id == currentUserId);

        if (user is null)
            throw new NotFoundException($"{nameof(UserDTO)} with Id {currentUserId}");

        var userRoles = (await _userManager.GetRolesAsync(user)).ToList();

        var userResult = _mapper.Map<UserDTO>(user);

        userRoles.ForEach(r =>
        {
            userResult.UserRoles.Add(r);
        });

        return userResult;
    }
    public async Task<UserDTO> GetUserByIdAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
            throw new NotFoundException($"{nameof(UserDTO)} with Id {id}");

        if (user.IsBanned)
        {
            throw new BadRequestException("User is banned");
        }

        var userRoles = (await _userManager.GetRolesAsync(user)).ToList();

        var userResult = _mapper.Map<UserDTO>(user);

        userRoles.ForEach(r =>
        {
            userResult.UserRoles.Add(r);
        });

        return userResult;
    }

    public async Task<IEnumerable<UserDTO>> SearchUsersAsync(string userNameOrEmailOrFullName)
    {
        var users = _mapper.Map<IEnumerable<UserDTO>>(
                  await _unitOfWork.Users.GetAsync(x =>
                     (x.UserName.Contains(userNameOrEmailOrFullName) ||
                     x.Email.Contains(userNameOrEmailOrFullName) ||
                     (x.FirstName + ' ' + x.LastName).Contains(userNameOrEmailOrFullName)) && !x.IsBanned, null, x => x.Include(x => x.UserRoles))
                 );

        return users;
    }
    public async Task<IEnumerable<UserDTO>> SearchSingersAsync(SearchDTO search)
    {
        var users = (await _userManager.GetUsersInRoleAsync(RoleConstants.SingerRole))
            .Where(x => (x.UserName.Contains(search.SearchTerm) || (x.FirstName + ' ' + x.LastName).Contains(search.SearchTerm)) && !x.IsBanned);

        return _mapper.Map<IEnumerable<UserDTO>>(users);
    }
    public async Task<IEnumerable<UserDTO>> GetUserSingersAsync(Guid id)
    {
        var singerIds = (await _unitOfWork.UserSongs.GetAsync(
            filter: x => x.UserId == id,
            includes: x => x.Include(x => x.Song))).Select(x => x.Song.SingerId);

        var singers = await _unitOfWork.Users.GetAsync(
            filter: x => singerIds.Contains(x.Id) && !x.IsBanned,
            orderBy: x => x.OrderBy(x=> x.UserName));

        return _mapper.Map<IEnumerable<UserDTO>>(singers);
    }



    public async Task<bool> UpdateUserAsync(UpdateUserDTO userModel, Guid currentUserId)
    {
        var user = await _userManager.FindByIdAsync(currentUserId.ToString());

        if (user == null) throw new NotFoundException($"User with {currentUserId}");


        if (user.UserName != userModel.UserName.Trim())
        {
            var checkIfUsernameExists = _userManager.FindByNameAsync(userModel.UserName);
            if (checkIfUsernameExists is not null) throw new BadRequestException("Such username already exists");
        }

        if (user.Email != userModel.Email.Trim())
        {
            var checkIfEmailExists = _userManager.FindByEmailAsync(userModel.Email);
            if (checkIfEmailExists is not null) throw new BadRequestException("Such email already exists");
        }

        user.FirstName = userModel.FirstName;
        user.LastName = userModel.LastName;
        user.AboutMe = userModel.AboutMe;
        user.Email = userModel.Email;

        var claims = await _userManager.GetClaimsAsync(user);

        await _userManager.ReplaceClaimAsync(user, claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName),
            new Claim(ClaimTypes.GivenName, user.FirstName + ' ' + user.LastName));
        await _userManager.ReplaceClaimAsync(user, claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name),
            new Claim(JwtClaimTypes.Name, user.UserName));
        await _userManager.ReplaceClaimAsync(user, claims.FirstOrDefault(x => x.Type == ClaimTypes.Email),
            new Claim(ClaimTypes.Email, user.Email));

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded;
    }

    public async Task<string> UpdateUserImgAsync(IFormFile userImg, Guid currentUserId)
    {
        if (userImg is null) throw new NoFileException($"File is required! Please add file");

        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(x => x.Id == currentUserId);

        var img = await _fileService.GetFilePathAsync(userImg);

        if (user.UserImgUrl != null) _fileService.DeleteFile(user.UserImgUrl);

        user.UserImgUrl = img;
        _unitOfWork.Users.Update(user);

        await _unitOfWork.SaveAsync();


        return _hostSettings.CurrentHost + user.UserImgUrl;
    }

    public async Task AddUserRole(UserChangeRoleDTO roleModel)
    {
        var user = await _userManager.FindByIdAsync(roleModel.UserId);

        var allRoles = _roleManager.Roles.Where(x => x.Name != roleModel.RoleName);

        if(allRoles.Count() == 3)// 3 - all roles
        {
            throw new NotFoundException($"{roleModel.RoleName}" );
        }

        var role = await _roleManager.FindByNameAsync(roleModel.RoleName);
        
        await _userManager.AddToRoleAsync(user, role.Name);

        var claims = await _userManager.GetClaimsAsync(user);

        await _userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Role, role.Name));
    }

    public async Task BecomeSinger(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null) throw new NotFoundException($"User with id - {id} ");

        user.WantToBeSinger = true;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);

            throw new BadRequestException(string.Join(", ", errors));
        }
     
    }
    public async Task DeleteAcountAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null) throw new NotFoundException($"User with id - {id} ");

        user.IsBanned = true;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);

            throw new BadRequestException(string.Join(", ", errors));
        }
    }

}
