using iMusic.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iMusic.DAL.DataContext.Configurations;

public class UserAlbumConfiguration : IEntityTypeConfiguration<UserAlbum>
{
    public void Configure(EntityTypeBuilder<UserAlbum> builder)
    {
        builder.HasKey(x => new { x.UserId, x.AlbumId });

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserAlbums)
            .HasForeignKey(x => x.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Album)
            .WithMany(x => x.UserAlbums)
            .HasForeignKey(x => x.AlbumId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
