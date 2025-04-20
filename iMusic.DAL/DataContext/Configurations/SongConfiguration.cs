using iMusic.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iMusic.DAL.DataContext.Configurations;

public class SongConfiguration : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne<Category>(t => t.Category)
            .WithMany(t => t.Songs)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Album)
            .WithMany(x => x.Songs)
            .HasForeignKey(x => x.AlbumId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Singer)
         .WithMany(x => x.Songs)
         .HasForeignKey(x => x.SingerId)
         .IsRequired(false)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
