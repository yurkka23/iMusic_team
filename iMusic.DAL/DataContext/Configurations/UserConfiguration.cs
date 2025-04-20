using iMusic.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iMusic.DAL.DataContext.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Email).IsUnique();

        builder.HasOne(t => t.FavoriteList)
            .WithOne(t => t.User)
            .HasForeignKey<FavoriteList>(t => t.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Playlists)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.AuthorId)
             .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired(false)
            .IsRequired();
    }
}
