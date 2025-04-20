using iMusic.BL.Interfaces;
using iMusic.DAL.Constants;
using iMusic.DAL.DTOs.Search;
using iMusic.DAL.DTOs.Song;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iMusic.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SongController : BaseController
{
    private readonly ISongService _songService;
    public SongController(ISongService songService)
    {
        _songService = songService;
    }
   
    [Authorize(Roles = RoleConstants.AdminRole + "," + RoleConstants.SingerRole)]
    [HttpPost("upload-song")]
    public async Task<IActionResult> UploadSong([FromForm] UploadSongDTO songDTO)
    {
        await _songService.UploadSongAsync(UserId, songDTO);

        return Ok();
    }

    [Authorize(Roles = RoleConstants.AdminRole + "," + RoleConstants.SingerRole)]
    [HttpPut("edit-song")]
    public async Task<IActionResult> EditSong([FromForm] EditSongDTO songDTO)
    {
        await _songService.EditSongAsync(songDTO);

        return Ok();
    }

    [Authorize(Roles = RoleConstants.AdminRole + "," + RoleConstants.SingerRole)]
    [HttpDelete("delete-song")]
    public async Task<IActionResult> DeleteSong(Guid id)
    {
        var res = await _songService.DeleteSongAsync(id);

        return Ok(res);
    }

    [HttpGet("get-song")]
    public async Task<IActionResult> GetSong(Guid id)
    {
        var res = await _songService.GetSongAsync(id);

        return Ok(res);
    }

    [HttpGet("get-songs-by-singer")]
    public async Task<IActionResult> GetSongsBySinger(Guid id)
    {
        var res = await _songService.GetSongsBySinger(id);

        return Ok(res);
    }
    [Authorize]
    [HttpGet("get-user-songs")]
    public async Task<IActionResult> GetUserSongs()
    {
        var res = await _songService.GetUserSongsAsync(UserId);

        return Ok(res);
    }

    [HttpGet("recommends-songs-to-user")]
    public async Task<IActionResult> GetRecommendSongsToUser(Guid id)
    {
        var res = await _songService.GetRecommendSongsToUserAsync(id);

        return Ok(res);
    }

    [HttpGet("get-songs")]
    public async Task<IActionResult> GetSongs()
    {
        var res = await _songService.GetSongsAsync();

        return Ok(res);
    }

    [HttpGet("get-top-songs")]
    public async Task<IActionResult> GetTopSongs()
    {
        var res = await _songService.GetTopSongsAsync();

        return Ok(res);
    }
    [HttpGet("get-new-songs")]
    public async Task<IActionResult> GetNewSongs()
    {
        var res = await _songService.GetNewSongsAsync();

        return Ok(res);
    }

    [HttpGet("get-songs-by-category")]
    public async Task<IActionResult> GetSongsByCategory(Guid id)
    {
        var res = await _songService.GetSongsByCategoryAsync(id);

        return Ok(res);
    }

    [HttpGet("get-top-singer-songs")]
    public async Task<IActionResult> GetTopSingerSongs(Guid id)
    {
        var res = await _songService.GetTopSingerSongsAsync(id);

        return Ok(res);
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchSongs(SearchDTO request)
    {
        var res = await _songService.SearchSongsAsync(request);

        return Ok(res);
    }

    [Authorize]
    [HttpDelete("remove-song-form-user")]
    public async Task<IActionResult> RemoveSongFromUser(Guid songId)
    {
        var res = await _songService.RemoveSongFromUserAsync(UserId, songId);

        return Ok(res);
    }

    [Authorize]
    [HttpGet("add-song-to-user")]
    public async Task<IActionResult> AddSongToUser(Guid songId)
    {
        var res = await _songService.AddSongToUserAsync(UserId, songId);

        return Ok(res);
    }

    [Authorize]
    [HttpGet("get-user-recently-songs")]
    public async Task<IActionResult> GetUserRecentlySongs()
    {
        var res = await _songService.GetUserRecentlySongsAsync(UserId);

        return Ok(res);
    }
}
