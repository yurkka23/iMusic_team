using iMusic.DAL.DTOs.Album;
using iMusic.DAL.DTOs.Search;
using iMusic.DAL.DTOs.Song;

namespace iMusic.BL.Interfaces;

public interface IAlbumService
{
    Task<Guid> CreateAlbumAsync(Guid currentUserId, CreateAlbumDTO createAlbumDTO);
    Task<bool> EditAlbumAsync(EditAlbumDTO editAlbumDTO);
    Task<bool> DeleteAlbumAsync(Guid albumId);
    Task AddSongToAlbum(Guid currentUserId, UploadSongDTO uploadSongDTO);
    Task RemoveSongFromAlbum(Guid albumId, Guid songId);
    Task<AlbumDTO> GetAlbum(Guid id);
    Task<IEnumerable<AlbumDTO>> GetAlbums();
    Task<IEnumerable<AlbumDTO>> GetSingerAlbums(Guid singerId);
    Task<IEnumerable<AlbumDTO>> GetRecommendsAlbum();
    Task<IEnumerable<AlbumDTO>> SearchAlbumsAsync(SearchDTO search);
    Task<IEnumerable<AlbumDTO>> GetUserAlbumsAsync(Guid id);
    Task<bool> AddAlbumToUserAsync(Guid userId, Guid albumId);
    Task<bool> RemoveAlbumFromUserAsync(Guid userId, Guid albumId);
    Task<IEnumerable<AlbumDTO>> GetUserRecentlyAlbumsAsync(Guid id);


}
