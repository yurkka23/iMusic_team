using iMusic.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iMusic.DAL.DataContext.Configurations;

public class SongPlaylistConfiguration : IEntityTypeConfiguration<SongPlaylist>
{
    public void Configure(EntityTypeBuilder<SongPlaylist> builder)
    {
        builder.HasKey(x => new {x.SongId, x.PlaylistId});

        builder.HasOne(x => x.Song)
            .WithMany(x => x.SongPlaylists)
            .HasForeignKey(x => x.SongId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Playlist)
            .WithMany(x => x.SongPlaylists)
            .HasForeignKey(x => x.PlaylistId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
