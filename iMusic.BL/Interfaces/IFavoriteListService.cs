using iMusic.DAL.DTOs.FavoriteList;

namespace iMusic.BL.Interfaces;

public interface IFavoriteListService
{
    Task<FavoriteListDTO> GetFavoriteListAsync(Guid userId);
    Task<bool> AddSongToFavoriteListAsync(Guid songId, Guid listId);
    Task<bool> AddAlbumToFavoriteListAsync(Guid albumId, Guid listId);
    Task<bool> AddPlaylistToFavoriteListAsync(Guid playlistId, Guid listId);
    Task<bool> RemoveSongFromFavoriteListAsync(Guid songId, Guid listId);
    Task<bool> RemovePlaylistFromFavoriteListAsync(Guid playlistId, Guid listId);
    Task<bool> RemoveAlbumFromFavoriteListAsync(Guid albumId, Guid listId);
}
