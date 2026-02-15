using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulAi.Domain.Entities;

namespace ZulAi.Infrastructure.Data.Configurations;

public class InteractionConfiguration : IEntityTypeConfiguration<Interaction>
{
    public void Configure(EntityTypeBuilder<Interaction> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Type).HasConversion<int>();
        builder.Property(i => i.Description).HasMaxLength(500);
        builder.Property(i => i.OccurredAt).HasColumnType("datetime(6)");

        builder.HasIndex(i => new { i.UniverseStateId, i.Tick });
    }
}
