using Microsoft.EntityFrameworkCore;
using ZulAi.Domain.Entities;

namespace ZulAi.Infrastructure.Data;

public class ZulAiDbContext : DbContext
{
    public ZulAiDbContext(DbContextOptions<ZulAiDbContext> options) : base(options) { }

    public DbSet<UniverseState> UniverseStates => Set<UniverseState>();
    public DbSet<Atom> Atoms => Set<Atom>();
    public DbSet<AtomConnection> AtomConnections => Set<AtomConnection>();
    public DbSet<Interaction> Interactions => Set<Interaction>();
    public DbSet<GeneratedOutput> GeneratedOutputs => Set<GeneratedOutput>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ZulAiDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
