using Microsoft.AspNetCore.Identity;

namespace iMusic.DAL.Entities;

public class AppRole : IdentityRole<Guid>
{
    public ICollection<AppUserRole> UserRoles { get; set; }
}