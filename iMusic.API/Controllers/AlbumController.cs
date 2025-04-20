using iMusic.BL.Interfaces;
using iMusic.DAL.DTOs.Album;
using iMusic.DAL.DTOs.Song;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using iMusic.DAL.Constants;
using iMusic.DAL.DTOs.Search;

namespace iMusic.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlbumController : BaseController
{
    private readonly IAlbumService _albumService;
    public AlbumController(IAlbumService albumService)
    {
        _albumService = albumService;
    }

    [HttpGet("get-album")]
    public async Task<IActionResult> GetAlbum(Guid id)
    {
        var result = await _albumService.GetAlbum(id);

        return Ok(result);
    }
    [HttpGet("get-albums")]
    public async Task<IActionResult> GetAlbums()
    {
        var result = await _albumService.GetAlbums();

        return Ok(result);
    }
    [HttpGet("get-albums-by-singer")]
    public async Task<IActionResult> GetAlbumsBySinger(Guid singerId)
    {
        var result = await _albumService.GetSingerAlbums(singerId);

        return Ok(result);
    }

    [Authorize(Roles = RoleConstants.AdminRole + "," + RoleConstants.SingerRole)]
    [HttpPost("created-album")]
    public async Task<IActionResult> CreateAlbum([FromForm] CreateAlbumDTO createAlbum)
    {
        return Ok(await _albumService.CreateAlbumAsync(UserId, createAlbum));
    }

    [Authorize(Roles = RoleConstants.AdminRole + "," + RoleConstants.SingerRole)]
    [HttpPut("edit-album")]
    public async Task<IActionResult> EditAlbum([FromForm] EditAlbumDTO editAlbum)
    {
        return Ok(await _albumService.EditAlbumAsync(editAlbum));
    }

    [Authorize(Roles = RoleConstants.AdminRole + "," + RoleConstants.SingerRole)]
    [HttpDelete("delete-album")]
    public async Task<IActionResult> DeleteAlbum(Guid id)
    {
        return Ok(await _albumService.DeleteAlbumAsync(id));
    }

    [Authorize(Roles = RoleConstants.AdminRole + "," + RoleConstants.SingerRole)]
    [HttpPatch("add-song")]
    public async Task<IActionResult> AddSong([FromForm] UploadSongDTO uploadSong)
    {
        await _albumService.AddSongToAlbum(UserId, uploadSong);

        return Ok();
    }

    [Authorize(Roles = RoleConstants.AdminRole + "," + RoleConstants.SingerRole)]
    [HttpPatch("remove-song")]
    public async Task<IActionResult> RemoveSong(Guid albumId, Guid songId)
    {
        await _albumService.RemoveSongFromAlbum(albumId, songId);

        return Ok();
    }

    [HttpGet("get-album-by-id")]
    public async Task<IActionResult> GetAlbumById(Guid albumId)
    {
        var res = await _albumService.GetAlbum(albumId);

        return Ok(res);
    }

    [HttpGet("recommends-albums")]
    public async Task<IActionResult> GetRecommendsAlbums()
    {
        var res = await _albumService.GetRecommendsAlbum();

        return Ok(res);
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchAlbums(SearchDTO request)
    {
        var res = await _albumService.SearchAlbumsAsync(request);

        return Ok(res);
    }

    [Authorize]
    [HttpGet("get-user-abums")]
    public async Task<IActionResult> GetUserAlbums()
    {
        var res = await _albumService.GetUserAlbumsAsync(UserId);

        return Ok(res);
    }

    [Authorize]
    [HttpDelete("remove-album-form-user")]
    public async Task<IActionResult> RemoveAlbumFromUser(Guid albumId)
    {
        var res = await _albumService.RemoveAlbumFromUserAsync(UserId, albumId);

        return Ok(res);
    }

    [Authorize]
    [HttpGet("add-album-to-user")]
    public async Task<IActionResult> AddAlbumToUser(Guid albumId)
    {
        var res = await _albumService.AddAlbumToUserAsync(UserId, albumId);

        return Ok(res);
    }
    [Authorize]
    [HttpGet("get-user-recently-albums")]
    public async Task<IActionResult> GetUserRecentlyAlbums()
    {
        var res = await _albumService.GetUserRecentlyAlbumsAsync(UserId);

        return Ok(res);
    }

}
