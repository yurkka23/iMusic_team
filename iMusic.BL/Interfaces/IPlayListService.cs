using iMusic.DAL.DTOs.Playlist;
using iMusic.DAL.DTOs.Search;

namespace iMusic.BL.Interfaces;

public interface IPlayListService
{
    Task<bool> CreatePlayListAsync(Guid currentUserId, CreatePlayListDTO createPlaylist);
    Task<bool> DeletePlaylistAsync(Guid playlistId);
    Task<bool> EditPlayListAsync(EditPlayListDTO editPlaylist);
    Task<bool> ChangeImageAsync(ChangePlaylistImageDTO changePlaylistImage);
    Task<bool> AddSongToPlaylistAsync(Guid playlistId, Guid songId);
    Task<bool> RemoveSongFromPlaylistAsync(Guid playlistId, Guid songId);
    Task<PlaylistDTO> GetPlaylistAsync(Guid playlistId);
    Task<IEnumerable<PlaylistDTO>> GetPlaylistsAsync(); 
    Task<IEnumerable<PlaylistDTO>> GetPlaylistsByUserAsync(Guid userId);
    Task<IEnumerable<PlaylistDTO>> GetTopPlaylistAsync();
    Task<IEnumerable<PlaylistDTO>> SearchPlaylistsAsync(SearchDTO search);
}
