using iMusic.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iMusic.DAL.DataContext.Configurations;

public class FavoriteAlbumsConfiguration : IEntityTypeConfiguration<FavoriteAlbums>
{
    public void Configure(EntityTypeBuilder<FavoriteAlbums> builder)
    {
        builder.HasKey(x => new { x.FavoritelistId, x.AlbumId });

        builder.HasOne(x => x.Album)
            .WithMany(x => x.FavoriteAlbums)
            .HasForeignKey(x => x.AlbumId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Favoritelist)
            .WithMany(x => x.Albums)
            .HasForeignKey(x => x.FavoritelistId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
