using iMusic.BL.Interfaces;
using iMusic.DAL.DTOs.Playlist;
using iMusic.DAL.DTOs.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iMusic.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlaylistController : BaseController
{
    private readonly IPlayListService _playlistService;
    public PlaylistController(IPlayListService playlistService)
    {
        _playlistService = playlistService;
    }

    [HttpGet("get-playlist")]
    public async Task<IActionResult> GetPlaylist(Guid id)
    {
        var result = await _playlistService.GetPlaylistAsync(id);

        return Ok(result);
    }

    [HttpGet("get-top-playlist")]
    public async Task<IActionResult> GetTopPlaylist()
    {
        var result = await _playlistService.GetTopPlaylistAsync();

        return Ok(result);
    }

    [HttpGet("get-playlists")]
    public async Task<IActionResult> GetPlaylists()
    {
        var result = await _playlistService.GetPlaylistsAsync();

        return Ok(result);
    }

    [Authorize]
    [HttpGet("get-playlists-by-user")]
    public async Task<IActionResult> GetPlaylistsByUser()
    {
        var result = await _playlistService.GetPlaylistsByUserAsync(UserId);

        return Ok(result);
    }
    [HttpPost("search")]
    public async Task<IActionResult> SearchPlaylists(SearchDTO request)
    {
        var res = await _playlistService.SearchPlaylistsAsync(request);

        return Ok(res);
    }

    [Authorize]
    [HttpPost("created-playlist")]
    public async Task<IActionResult> CreatePlaylist([FromForm] CreatePlayListDTO createPlaylist)
    {
        return Ok(await _playlistService.CreatePlayListAsync(UserId, createPlaylist));
    }

    [Authorize]
    [HttpPut("edit-playlist")]
    public async Task<IActionResult> EditPlaylist([FromForm] EditPlayListDTO editPlaylist)
    {
        return Ok(await _playlistService.EditPlayListAsync(editPlaylist));
    }

    [Authorize]
    [HttpDelete("delete-playlist")]
    public async Task<IActionResult> DeletePlaylist(Guid id)
    {
        return Ok(await _playlistService.DeletePlaylistAsync(id));
    }

    [Authorize]
    [HttpPatch("change-image")]
    public async Task<IActionResult> ChangeImageAsync([FromForm] ChangePlaylistImageDTO changePlaylistImage)
    {
        return Ok(await _playlistService.ChangeImageAsync(changePlaylistImage));
    }

    [Authorize]
    [HttpGet("add-song")]
    public async Task<IActionResult> AddSongAsync(Guid playlistId, Guid songId)
    {
        return Ok(await _playlistService.AddSongToPlaylistAsync(playlistId, songId));
    }

    [Authorize]
    [HttpGet("remove-song")]
    public async Task<IActionResult> RemoveSongAsync(Guid playlistId, Guid songId)
    {
        return Ok(await _playlistService.RemoveSongFromPlaylistAsync(playlistId, songId));
    }
}
