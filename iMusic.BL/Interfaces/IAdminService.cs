using iMusic.DAL.DTOs.Album;
using iMusic.DAL.DTOs.FavoriteList;
using iMusic.DAL.DTOs.Playlist;
using iMusic.DAL.DTOs.Song;
using iMusic.DAL.DTOs.User;

namespace iMusic.BL.Interfaces;

public interface IAdminService
{
    Task<UserDTO> GetUserInfoAsync(Guid id);
    Task<IEnumerable<SongDTO>> GetUserAddedSongsAsync(Guid id);
    Task<IEnumerable<AlbumDTO>> GetUserAddedAlbumsAsync(Guid id);
    Task<FavoriteListDTO> GetUserFavoritelistAsync(Guid userId);
    Task<IEnumerable<PlaylistDTO>> GetUserPlaylistsAsync(Guid userId);

    Task<IEnumerable<UserDTO>> GetBecomeSingerRequestsAsync();
    Task<IEnumerable<UserDTO>> GetSingersAsync();
    Task<IEnumerable<UserDTO>> GetUsersAsync();
    Task<IEnumerable<UserDTO>> GetBannedUsersAsync();
    Task ApproveSingerAsync(Guid id);
    Task RejectSingerAsync(Guid id);
    Task RemoveSingerRoleAsync(Guid id);
    Task BanUserAsync(Guid id);
    Task UnBanUserAsync(Guid id);
}
