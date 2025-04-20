using iMusic.BL.Interfaces;
using iMusic.DAL.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using iMusic.DAL.DTOs.User;
using iMusic.DAL.DTOs.Search;

namespace iMusic.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : BaseController
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GerUserInformation()
    {
        var res = await _userService.GetCurrentUserInformationAsync(UserId);
        return Ok(res);
    }

    [Authorize(Roles = RoleConstants.AdminRole)]
    [HttpGet("users/{userNameOrEmailOrFullName}")]
    public async Task<IActionResult> SearchUsersInformation(string userNameOrEmailOrFullName)
    {
        var res = await _userService.SearchUsersAsync(userNameOrEmailOrFullName);
        return Ok(res);
    }

    [HttpGet("get-user")]
    public async Task<IActionResult> GetUserById(Guid id)
    {

        var res = await _userService.GetUserByIdAsync(id);
        return Ok(res);
    }

    [HttpPost("search-singer")]
    public async Task<IActionResult> SearchSingers(SearchDTO request)
    {
        var res = await _userService.SearchSingersAsync(request);

        return Ok(res);
    }

    [Authorize]
    [HttpGet("get-user-singers")]
    public async Task<IActionResult> GetUserSingers()
    {
        var res = await _userService.GetUserSingersAsync(UserId);

        return Ok(res);
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser(UpdateUserDTO userModel)
    {

        var res = await _userService.UpdateUserAsync(userModel, UserId);
        return NoContent();
    }

    [Authorize]
    [HttpPut("update-user-profile-image")]
    public async Task<IActionResult> UpdateUserImg(IFormFile userProfileImage)
    {
        var link = await _userService.UpdateUserImgAsync(userProfileImage, UserId);

        return Ok(new { urlPhoto = link });
    }

    [Authorize(Roles = RoleConstants.AdminRole)]
    [HttpPut("add-user-role")]
    public async Task<IActionResult> AddUserRole(UserChangeRoleDTO roleModel)
    {
        await _userService.AddUserRole(roleModel);

        return Ok();
    }

    [Authorize]
    [HttpGet("become-singer")]
    public async Task<IActionResult> BecomeSinger(Guid id)
    {
        await _userService.BecomeSinger(id);

        return Ok();
    }
    [Authorize]
    [HttpDelete("delete-acount")]
    public async Task<IActionResult> DeleteAcount()
    {
        await _userService.DeleteAcountAsync(UserId);

        return Ok();
    }
}
