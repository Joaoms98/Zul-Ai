using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulAi.Domain.Entities;

namespace ZulAi.Infrastructure.Data.Configurations;

public class UniverseStateConfiguration : IEntityTypeConfiguration<UniverseState>
{
    public void Configure(EntityTypeBuilder<UniverseState> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.GridWidth).HasDefaultValue(80);
        builder.Property(u => u.GridHeight).HasDefaultValue(40);
        builder.Property(u => u.CreatedAt).HasColumnType("datetime(6)");
        builder.Property(u => u.LastTickAt).HasColumnType("datetime(6)");

        builder.HasMany(u => u.Atoms)
            .WithOne(a => a.UniverseState)
            .HasForeignKey(a => a.UniverseStateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Interactions)
            .WithOne(i => i.UniverseState)
            .HasForeignKey(i => i.UniverseStateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Outputs)
            .WithOne(o => o.UniverseState)
            .HasForeignKey(o => o.UniverseStateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
