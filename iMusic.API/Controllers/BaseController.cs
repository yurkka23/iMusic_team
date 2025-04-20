using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace iMusic.API.Controllers;
[Route("api/imusic")]
[ApiController]
public class BaseController : ControllerBase
{
    protected Guid UserId => !HttpContext.User.Identity.IsAuthenticated
            ? Guid.Empty
            : Guid.Parse(HttpContext.User.FindFirst(JwtClaimTypes.Id).Value);
    protected string UserName => !HttpContext.User.Identity.IsAuthenticated
        ? string.Empty
        : HttpContext.User.FindFirst(JwtClaimTypes.Name).Value;

    protected string Email => !HttpContext.User.Identity.IsAuthenticated
        ? string.Empty
        : HttpContext.User.FindFirst(ClaimTypes.Email).Value;

    protected string FullName => !HttpContext.User.Identity.IsAuthenticated
       ? string.Empty
       : HttpContext.User.FindFirst(ClaimTypes.GivenName).Value;
}
