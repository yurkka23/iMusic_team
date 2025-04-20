using iMusic.DAL.Constants;
using iMusic.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iMusic.DAL.DataContext.Configurations;

public class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.HasMany(ur => ur.UserRoles)
             .WithOne(u => u.Role)
             .HasForeignKey(ur => ur.RoleId)
             .IsRequired();

        //seed
        builder.HasData(new AppRole
        {
            Id = Guid.Parse("1CFBA6F6-50FA-4304-9140-4BD86F5A5885"),
            Name = RoleConstants.AdminRole,
            NormalizedName = RoleConstants.AdminRole.ToUpper()
        });

        builder.HasData(new AppRole
        {
            Id = Guid.Parse("D013CA6B-EA46-4947-9A7F-A249406FF873"),
            Name = RoleConstants.UserRole,
            NormalizedName = RoleConstants.UserRole.ToUpper()
        });

        builder.HasData(new AppRole
        {
            Id = Guid.Parse("70F964D3-7677-4AC5-952B-F4713352CA5E"),
            Name = RoleConstants.SingerRole,
            NormalizedName = RoleConstants.SingerRole.ToUpper()
        });
    }
}
