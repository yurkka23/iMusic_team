using iMusic.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iMusic.DAL.DataContext.Configurations;

public class FavoritePlaylistsConfiguration : IEntityTypeConfiguration<FavoritePlaylists>
{
    public void Configure(EntityTypeBuilder<FavoritePlaylists> builder)
    {
        builder.HasKey(x => new { x.FavoritelistId, x.PlaylistId });

        builder.HasOne(x => x.Playlist)
            .WithMany(x => x.FavoritePlaylists)
            .HasForeignKey(x => x.PlaylistId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Favoritelist)
            .WithMany(x => x.Playlists)
            .HasForeignKey(x => x.FavoritelistId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
