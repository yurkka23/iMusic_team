using iMusic.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iMusic.DAL.DataContext.Configurations;

public class UserSongConfiguration : IEntityTypeConfiguration<UserSong>
{
    public void Configure(EntityTypeBuilder<UserSong> builder)
    {
        builder.HasKey(x => new { x.UserId, x.SongId });

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserSongs)
            .HasForeignKey(x => x.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Song)
            .WithMany(x => x.UserSongs)
            .HasForeignKey(x => x.SongId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
