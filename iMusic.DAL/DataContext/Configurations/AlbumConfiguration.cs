using iMusic.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iMusic.DAL.DataContext.Configurations;

public class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(t => t.Category)
            .WithMany(t => t.Albums)
            .HasForeignKey(t => t.CategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Albums)
            .HasForeignKey(x => x.SingerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
