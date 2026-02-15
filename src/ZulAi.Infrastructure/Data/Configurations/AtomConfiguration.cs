using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulAi.Domain.Entities;

namespace ZulAi.Infrastructure.Data.Configurations;

public class AtomConfiguration : IEntityTypeConfiguration<Atom>
{
    public void Configure(EntityTypeBuilder<Atom> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Type).HasConversion<int>();
        builder.Property(a => a.Symbol).HasColumnType("char(1)");
        builder.Property(a => a.CreatedAt).HasColumnType("datetime(6)");

        builder.HasIndex(a => new { a.UniverseStateId, a.IsAlive });
    }
}
