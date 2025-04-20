using iMusic.DAL.DTOs.Search;
using iMusic.DAL.DTOs.User;
using Microsoft.AspNetCore.Http;

namespace iMusic.BL.Interfaces;

public interface IUserService
{
    Task<UserDTO> GetCurrentUserInformationAsync(Guid currentUserId);
    Task<UserDTO> GetUserByIdAsync(Guid id);
    Task<IEnumerable<UserDTO>> SearchUsersAsync(string userNameOrEmailOrFullName);
    Task<bool> UpdateUserAsync(UpdateUserDTO userModel, Guid currentUserId);
    Task<string> UpdateUserImgAsync(IFormFile userImg, Guid currentUserId);
    Task AddUserRole(UserChangeRoleDTO roleModel);
    Task BecomeSinger(Guid id);
    Task<IEnumerable<UserDTO>> SearchSingersAsync(SearchDTO search);
    Task<IEnumerable<UserDTO>> GetUserSingersAsync(Guid id);
    Task DeleteAcountAsync(Guid id);

}
