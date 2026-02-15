using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ZulAi.Infrastructure.Data;

public class ZulAiDbContextFactory : IDesignTimeDbContextFactory<ZulAiDbContext>
{
    public ZulAiDbContext CreateDbContext(string[] args)
    {
        var apiProjectPath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "src", "ZulAi.Api"));

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("ZulAiDb")!;
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));

        var optionsBuilder = new DbContextOptionsBuilder<ZulAiDbContext>();
        optionsBuilder.UseMySql(connectionString, serverVersion, options =>
        {
            options.MigrationsAssembly("ZulAi.Infrastructure");
        });

        return new ZulAiDbContext(optionsBuilder.Options);
    }
}
