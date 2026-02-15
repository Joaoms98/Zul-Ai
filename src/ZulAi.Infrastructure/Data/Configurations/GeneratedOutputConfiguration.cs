using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulAi.Domain.Entities;

namespace ZulAi.Infrastructure.Data.Configurations;

public class GeneratedOutputConfiguration : IEntityTypeConfiguration<GeneratedOutput>
{
    public void Configure(EntityTypeBuilder<GeneratedOutput> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.AsciiArt).HasColumnType("longtext");
        builder.Property(o => o.GeneratedAt).HasColumnType("datetime(6)");

        builder.HasIndex(o => new { o.UniverseStateId, o.Tick }).IsUnique();
    }
}
