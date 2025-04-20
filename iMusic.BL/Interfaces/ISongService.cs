using iMusic.DAL.DTOs.Search;
using iMusic.DAL.DTOs.Song;
using Microsoft.AspNetCore.Http;

namespace iMusic.BL.Interfaces;

public interface ISongService
{
    Task UploadSongAsync(Guid currentUserId, UploadSongDTO songDTO);
    Task EditSongAsync(EditSongDTO songDTO);
    Task<bool> DeleteSongAsync(Guid songId);
    Task<SongDTO> GetSongAsync(Guid songId);
    Task<IEnumerable<SongDTO>> GetSongsAsync(); 
    Task<IEnumerable<SongDTO>> GetSongsBySinger(Guid id);
    Task<IEnumerable<SongDTO>> GetTopSongsAsync(); 
    Task<IEnumerable<SongDTO>> GetNewSongsAsync();
    Task<IEnumerable<SongDTO>> GetRecommendSongsToUserAsync(Guid id);
    Task<IEnumerable<SongDTO>> GetSongsByCategoryAsync(Guid id);
    Task<IEnumerable<SongDTO>> GetTopSingerSongsAsync(Guid id);
    Task<IEnumerable<SongDTO>> SearchSongsAsync(SearchDTO search);
    Task<IEnumerable<SongDTO>> GetUserSongsAsync(Guid id);
    Task<bool> AddSongToUserAsync(Guid userId, Guid songId);
    Task<bool> RemoveSongFromUserAsync(Guid userId, Guid songId);
    Task<IEnumerable<SongDTO>> GetUserRecentlySongsAsync(Guid id);

}
