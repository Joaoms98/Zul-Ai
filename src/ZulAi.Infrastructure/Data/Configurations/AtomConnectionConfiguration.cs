using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulAi.Domain.Entities;

namespace ZulAi.Infrastructure.Data.Configurations;

public class AtomConnectionConfiguration : IEntityTypeConfiguration<AtomConnection>
{
    public void Configure(EntityTypeBuilder<AtomConnection> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.SourceAtom)
            .WithMany(a => a.ConnectionsAsSource)
            .HasForeignKey(c => c.SourceAtomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.TargetAtom)
            .WithMany(a => a.ConnectionsAsTarget)
            .HasForeignKey(c => c.TargetAtomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.SourceAtomId);
        builder.HasIndex(c => c.TargetAtomId);
    }
}
