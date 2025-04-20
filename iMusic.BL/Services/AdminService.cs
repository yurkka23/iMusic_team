using AutoMapper;
using IdentityModel;
using iMusic.BL.Exceptions;
using iMusic.BL.Interfaces;
using iMusic.DAL.Abstractions;
using iMusic.DAL.Constants;
using iMusic.DAL.DTOs.Album;
using iMusic.DAL.DTOs.FavoriteList;
using iMusic.DAL.DTOs.Playlist;
using iMusic.DAL.DTOs.Song;
using iMusic.DAL.DTOs.User;
using iMusic.DAL.Entities;
using iMusic.DAL.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace iMusic.BL.Services;

public class AdminService: IAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly HostSettings _hostSettings;
    private readonly RoleManager<AppRole> _roleManager;

    public AdminService(IUnitOfWork unitOfWork, IMapper mapper,
         UserManager<User> userManager, IOptionsSnapshot<HostSettings> hostSettings, RoleManager<AppRole> roleManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
        _hostSettings = hostSettings.Value;
        _roleManager = roleManager;
    }

    public async Task<UserDTO> GetUserInfoAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Id == id);

        return _mapper.Map<UserDTO>(user); ;
    }
    public async Task<IEnumerable<SongDTO>> GetUserAddedSongsAsync(Guid id)
    {
        var songs = (await _unitOfWork.UserSongs.GetAsync(
           filter: x => x.UserId == id,
           orderBy: x => x.OrderByDescending(x => x.AddedTime),
           includes: x => x.Include(x => x.Song).ThenInclude(s => s.Category)
           .Include(x => x.Song).ThenInclude(s => s.Singer)
           .Include(x => x.Song).ThenInclude(s => s.Album))).Select(x => x.Song);

        return _mapper.Map<IEnumerable<SongDTO>>(songs);

    }

    public async Task<IEnumerable<AlbumDTO>> GetUserAddedAlbumsAsync(Guid id)
    {
        var albumsIds = (await _unitOfWork.UserAlbums.GetAsync(
           filter: x => x.UserId == id,
           includes: x => x.Include(x => x.Album),
           orderBy: x => x.OrderByDescending(x => x.AddedTime))).Select(x => x.Album.Id);

        var albums = (await _unitOfWork.Albums.GetAsync(
            filter: x => albumsIds.Contains(x.Id),
            includes: a => a
            .Include(a => a.Category)
            .Include(a => a.User)
            .Include(a => a.Songs).ThenInclude(s => s.Singer)));

        return _mapper.Map<IEnumerable<AlbumDTO>>(albums);

    }
    public async Task<FavoriteListDTO> GetUserFavoritelistAsync(Guid userId)
    {
        var list = await _unitOfWork.FavoriteLists.GetFirstOrDefaultAsync(c => c.UserId == userId);

        var listSongs = await _unitOfWork.FavoriteSongs.GetAsync(c => c.FavoritelistId == userId,
            includes: x => x.Include(s => s.Song).ThenInclude(s => s.Singer)
            .Include(s => s.Song).ThenInclude(s => s.Category),
            orderBy: s => s.OrderByDescending(x => x.AddedTime));

        var listPlaylists = await _unitOfWork.FavoritePlaylists.GetAsync(c => c.FavoritelistId == userId,
            includes: x => x.Include(s => s.Playlist).ThenInclude(s => s.User),
             orderBy: s => s.OrderByDescending(x => x.AddedTime));

        var listAlbums = await _unitOfWork.FavoriteAlbums.GetAsync(c => c.FavoritelistId == userId,
           includes: x => x.Include(s => s.Album).ThenInclude(s => s.Category)
            .Include(s => s.Album).ThenInclude(s => s.User),
            orderBy: s => s.OrderByDescending(x => x.AddedTime));

        if (listSongs is not null && listSongs.Count() > 0)
        {
            list.Songs = (ICollection<FavoriteSongs>)listSongs;

        }

        if (listAlbums is not null && listAlbums.Count() > 0)
        {
            list.Albums = (ICollection<FavoriteAlbums>)listAlbums;

        }

        if (listPlaylists is not null && listPlaylists.Count() > 0)
        {
            list.Playlists = (ICollection<FavoritePlaylists>)listPlaylists;
        }


        if (list is null)
        {
            var newList = new FavoriteList()
            {
                UserId = userId
            };
            await _unitOfWork.FavoriteLists.InsertAsync(newList);

            await _unitOfWork.SaveAsync();

            return _mapper.Map<FavoriteListDTO>(newList);
        }


        return _mapper.Map<FavoriteListDTO>(list);
    }

    public async Task<IEnumerable<PlaylistDTO>> GetUserPlaylistsAsync(Guid userId)
    {
        var playlists = await _unitOfWork.Playlists.GetAsync(x => x.AuthorId == userId,
            includes: p => p.Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Singer)
            .Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Category)
            .Include(p => p.SongPlaylists).ThenInclude(s => s.Song).ThenInclude(s => s.Album)
            .Include(p => p.User));

        return _mapper.Map<IEnumerable<PlaylistDTO>>(playlists);
    }

    public async Task<IEnumerable<UserDTO>> GetBecomeSingerRequestsAsync()
    {
        var users = await _unitOfWork.Users.GetAsync(u => u.WantToBeSinger == true && !u.IsBanned);
   
        return _mapper.Map<IEnumerable<UserDTO>>(users); ;
    }

    public async Task<IEnumerable<UserDTO>> GetBannedUsersAsync()
    {
        var users = await _unitOfWork.Users.GetAsync(u => u.IsBanned);

        return _mapper.Map<IEnumerable<UserDTO>>(users); ;
    }

    public async Task<IEnumerable<UserDTO>> GetSingersAsync()
    {
        var users = (await _userManager.GetUsersInRoleAsync(RoleConstants.SingerRole)).Where(x => !x.IsBanned);

        return _mapper.Map<IEnumerable<UserDTO>>(users); ;
    }

    public async Task<IEnumerable<UserDTO>> GetUsersAsync()
    {
        var users = (await _userManager.GetUsersInRoleAsync(RoleConstants.UserRole)).Where(x => !x.IsBanned); ;

        return _mapper.Map<IEnumerable<UserDTO>>(users); ;
    }

    public async Task ApproveSingerAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null) throw new NotFoundException($"User with id - {id} ");

        var res = await _userManager.RemoveFromRoleAsync(user, RoleConstants.UserRole);

        if (!res.Succeeded)
        {
            var errors = res.Errors.Select(e => e.Description);

            throw new BadRequestException(string.Join(", ", errors));
        }

        var result = await _userManager.AddToRoleAsync(user, RoleConstants.SingerRole);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);

            throw new BadRequestException(string.Join(", ", errors));
        }

        var claims = await _userManager.GetClaimsAsync(user);

        await _userManager.RemoveClaimAsync(user, new Claim(JwtClaimTypes.Role, RoleConstants.UserRole));

        await _userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Role, RoleConstants.SingerRole));

        user.WantToBeSinger = null;

        var result1 = await _userManager.UpdateAsync(user);

        if (!result1.Succeeded)
        {
            var errors = result1.Errors.Select(e => e.Description);

            throw new BadRequestException(string.Join(", ", errors));
        }
    }

    public async Task RejectSingerAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null) throw new NotFoundException($"User with id - {id} ");

        user.WantToBeSinger = null;
     
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);

            throw new BadRequestException(string.Join(", ", errors));
        }
    }

    public async Task RemoveSingerRoleAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null) throw new NotFoundException($"User with id - {id} ");

        var res = await _userManager.RemoveFromRoleAsync(user, RoleConstants.SingerRole);

        if (!res.Succeeded)
        {
            var errors = res.Errors.Select(e => e.Description);

            throw new BadRequestException(string.Join(", ", errors));
        }

        var result = await _userManager.AddToRoleAsync(user, RoleConstants.UserRole);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);

            throw new BadRequestException(string.Join(", ", errors));
        }

        var claims = await _userManager.GetClaimsAsync(user);

        await _userManager.RemoveClaimAsync(user, new Claim(JwtClaimTypes.Role, RoleConstants.SingerRole));

        await _userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Role, RoleConstants.UserRole));


        var result1 = await _userManager.UpdateAsync(user);

        if (!result1.Succeeded)
        {
            var errors = result1.Errors.Select(e => e.Description);

            throw new BadRequestException(string.Join(", ", errors));
        }
    }

    public async Task BanUserAsync(Guid id)
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
    public async Task UnBanUserAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null) throw new NotFoundException($"User with id - {id} ");

        user.IsBanned = false;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);

            throw new BadRequestException(string.Join(", ", errors));
        }
    }
}
