using iMusic.BL.Interfaces;
using iMusic.DAL.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iMusic.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = RoleConstants.AdminRole)]
public class AdminController : BaseController
{
    private readonly IAdminService _admibService;

    public AdminController(IAdminService admibService)
    {
        _admibService = admibService;
    }

    [HttpGet("approve-singer")]
    public async Task<IActionResult> ApproveSinger(Guid id)
    {
        await _admibService.ApproveSingerAsync(id);

        return Ok();
    }

    [HttpGet("reject-singer")]
    public async Task<IActionResult> RejectSinger(Guid id)
    {
        await _admibService.RejectSingerAsync(id);

        return Ok();
    }
    [HttpGet("remove-singer-role")]
    public async Task<IActionResult> RemoveSingerRole(Guid id)
    {
        await _admibService.RemoveSingerRoleAsync(id);

        return Ok();
    }
    [HttpGet("ban")]
    public async Task<IActionResult> BanUser(Guid id)
    {
        await _admibService.BanUserAsync(id);

        return Ok();
    }
    [HttpGet("unban")]
    public async Task<IActionResult> UnBanUser(Guid id)
    {
        await _admibService.UnBanUserAsync(id);

        return Ok();
    }

    [HttpGet("get-become-singer-requests")]
    public async Task<IActionResult> GetBecomeSingerRequests()
    {
        var res = await _admibService.GetBecomeSingerRequestsAsync();

        return Ok(res);
    }

    [HttpGet("get-singers")]
    public async Task<IActionResult> GetSingers()
    {
        var res = await _admibService.GetSingersAsync();

        return Ok(res);
    }

    [HttpGet("get-users")]
    public async Task<IActionResult> GetUsers()
    {
        var res = await _admibService.GetUsersAsync();

        return Ok(res);
    }

    [HttpGet("get-banned-users")]
    public async Task<IActionResult> GetBannedUsers()
    {
        var res = await _admibService.GetBannedUsersAsync();

        return Ok(res);
    }

    [HttpGet("get-user-info")]
    public async Task<IActionResult> GetUserInfo(Guid id)
    {
        var res = await _admibService.GetUserInfoAsync(id);

        return Ok(res);
    }

    [HttpGet("get-user-added-songs")]
    public async Task<IActionResult> GetUserAddedSongs(Guid id)
    {
        var res = await _admibService.GetUserAddedSongsAsync(id);

        return Ok(res);
    }

    [HttpGet("get-user-added-albums")]
    public async Task<IActionResult> GetUserAddedAlbums(Guid id)
    {
        var res = await _admibService.GetUserAddedAlbumsAsync(id);

        return Ok(res);
    }
    [HttpGet("get-user-favoritelist")]
    public async Task<IActionResult> GetUserFavoritelist(Guid id)
    {
        var res = await _admibService.GetUserFavoritelistAsync(id);

        return Ok(res);
    }

    [HttpGet("get-user-playlists")]
    public async Task<IActionResult> GetUserPlaylists(Guid id)
    {
        var res = await _admibService.GetUserPlaylistsAsync(id);

        return Ok(res);
    }


}
