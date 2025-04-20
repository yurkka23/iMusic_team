using iMusic.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iMusic.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FavoriteListController : BaseController
{
    private readonly IFavoriteListService _favoriteListService;
    public FavoriteListController(IFavoriteListService favoriteListService)
    {
        _favoriteListService = favoriteListService;
    }

    [HttpGet("get-favorite-list")]
    public async Task<IActionResult> GetFavoriteList()
    {
        var result = await _favoriteListService.GetFavoriteListAsync(UserId);

        return Ok(result);
    }
  
    [HttpGet("add-song")]
    public async Task<IActionResult> AddSong( Guid songId)
    {
        return Ok(await _favoriteListService.AddSongToFavoriteListAsync(songId, UserId));
    }

    [HttpGet("add-playlist")]
    public async Task<IActionResult> AddPlaylist( Guid playlistId)
    {
        return Ok(await _favoriteListService.AddPlaylistToFavoriteListAsync(playlistId, UserId));
    }

    [HttpGet("add-album")]
    public async Task<IActionResult> AddAlbum( Guid albumId)
    {
        return Ok(await _favoriteListService.AddAlbumToFavoriteListAsync(albumId, UserId));
    }

    [HttpGet("remove-song")]
    public async Task<IActionResult> RemoveSong( Guid songId)
    {
        return Ok(await _favoriteListService.RemoveSongFromFavoriteListAsync(songId, UserId));
    }

    [HttpGet("remove-playlist")]
    public async Task<IActionResult> RemovePlaylist(Guid playlistId)
    {
        return Ok(await _favoriteListService.RemovePlaylistFromFavoriteListAsync(playlistId, UserId));
    }

    [HttpGet("remove-album")]
    public async Task<IActionResult> RemoveAlbum( Guid albumId)
    {
        return Ok(await _favoriteListService.RemoveAlbumFromFavoriteListAsync(albumId, UserId));
    }
}
