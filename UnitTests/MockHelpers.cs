using Microsoft.AspNetCore.Identity;
using Moq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;

namespace UnitTests;

public static class MockHelpers
{
    public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        var userManager = new Mock<UserManager<TUser>>(
            store.Object, null, null, null, null, null, null, null, null);
        userManager.Object.UserValidators.Add(new UserValidator<TUser>());
        userManager.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

        userManager.Setup(um => um.DeleteAsync(It.IsAny<TUser>()))
            .ReturnsAsync(IdentityResult.Success);
        userManager.Setup(um => um.UpdateAsync(It.IsAny<TUser>()))
            .ReturnsAsync(IdentityResult.Success);
        userManager.Setup(um => um.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success)
            .Callback<TUser, string>((user, password) => { /* Optionally track created users */ });
        userManager.Setup(um => um.AddToRoleAsync(It.IsAny<TUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        userManager.Setup(um => um.RemoveFromRoleAsync(It.IsAny<TUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        userManager.Setup(um => um.GetRolesAsync(It.IsAny<TUser>()))
            .ReturnsAsync(new List<string>());
        userManager.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string userId) => null); // Configure as needed
        userManager.Setup(um => um.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((string userName) => null); // Configure as needed
        userManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) => null); // Configure as needed
        userManager.Setup(um => um.GetClaimsAsync(It.IsAny<TUser>()))
            .ReturnsAsync(new List<System.Security.Claims.Claim>());
        userManager.Setup(um => um.AddClaimAsync(It.IsAny<TUser>(), It.IsAny<System.Security.Claims.Claim>()))
            .ReturnsAsync(IdentityResult.Success);
        userManager.Setup(um => um.ReplaceClaimAsync(It.IsAny<TUser>(), It.IsAny<System.Security.Claims.Claim>(), It.IsAny<System.Security.Claims.Claim>()))
            .ReturnsAsync(IdentityResult.Success);
        userManager.Setup(um => um.RemoveClaimAsync(It.IsAny<TUser>(), It.IsAny<System.Security.Claims.Claim>()))
            .ReturnsAsync(IdentityResult.Success);
        userManager.Setup(um => um.CheckPasswordAsync(It.IsAny<TUser>(), It.IsAny<string>()))
            .ReturnsAsync(true); // Or false based on your test

        return userManager;
    }

    public static Mock<RoleManager<TRole>> MockRoleManager<TRole>() where TRole : class
    {
        var store = new Mock<IRoleStore<TRole>>();
        var roles = new List<TRole>();
        var roleManager = new Mock<RoleManager<TRole>>(
            store.Object, null, null, null, null);

        roleManager.Setup(rm => rm.CreateAsync(It.IsAny<TRole>()))
            .ReturnsAsync(IdentityResult.Success)
            .Callback<TRole>(r => roles.Add(r));
        roleManager.Setup(rm => rm.DeleteAsync(It.IsAny<TRole>()))
            .ReturnsAsync(IdentityResult.Success)
            .Callback<TRole>(r => roles.Remove(r));
        roleManager.Setup(rm => rm.UpdateAsync(It.IsAny<TRole>()))
            .ReturnsAsync(IdentityResult.Success);
        roleManager.Setup(rm => rm.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string roleId) => roles.FirstOrDefault(r => GetRoleId(r).ToString() == roleId));
        roleManager.Setup(rm => rm.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((string roleName) => roles.FirstOrDefault(r => GetRoleName(r) == roleName));
        roleManager.Setup(rm => rm.Roles).Returns(roles.AsQueryable());

        // Helper methods to access properties using reflection (replace with actual property access if possible)
        Guid GetRoleId(TRole role) => (Guid)role.GetType().GetProperty("Id")?.GetValue(role);
        string GetRoleName(TRole role) => role.GetType().GetProperty("Name")?.GetValue(role)?.ToString();

        return roleManager;
    }
}
