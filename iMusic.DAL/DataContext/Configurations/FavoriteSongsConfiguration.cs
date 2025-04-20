using iMusic.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iMusic.DAL.DataContext.Configurations;

public class FavoriteSongsConfiguration : IEntityTypeConfiguration<FavoriteSongs>
{
    public void Configure(EntityTypeBuilder<FavoriteSongs> builder)
    {
        builder.HasKey(x => new { x.FavoritelistId, x.SongId });

        builder.HasOne(x => x.Song)
            .WithMany(x => x.FavoriteSongs)
            .HasForeignKey(x => x.SongId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Favoritelist)
            .WithMany(x => x.Songs)
            .HasForeignKey(x => x.FavoritelistId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
