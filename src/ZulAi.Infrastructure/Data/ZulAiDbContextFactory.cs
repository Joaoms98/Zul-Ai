using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ZulAi.Infrastructure.Data;

public class ZulAiDbContextFactory : IDesignTimeDbContextFactory<ZulAiDbContext>
{
    public ZulAiDbContext CreateDbContext(string[] args)
    {
        var apiProjectPath = FindApiProjectPath();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .Build();

        var connectionString = configuration.GetConnectionString("ZulAiDb")!;
        var serverVersion = ServerVersion.AutoDetect(connectionString);

        var optionsBuilder = new DbContextOptionsBuilder<ZulAiDbContext>();
        optionsBuilder.UseMySql(connectionString, serverVersion, options =>
        {
            options.MigrationsAssembly("ZulAi.Infrastructure");
        });

        return new ZulAiDbContext(optionsBuilder.Options);
    }

    private static string FindApiProjectPath()
    {
        var cwd = Directory.GetCurrentDirectory();

        // If we're already inside ZulAi.Api (dotnet run)
        if (File.Exists(Path.Combine(cwd, "appsettings.json"))
            && File.Exists(Path.Combine(cwd, "ZulAi.Api.csproj")))
        {
            return cwd;
        }

        // Walk up from cwd looking for the Api project
        var dir = new DirectoryInfo(cwd);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, "src", "ZulAi.Api");
            if (Directory.Exists(candidate)
                && File.Exists(Path.Combine(candidate, "appsettings.json")))
            {
                return candidate;
            }
            dir = dir.Parent;
        }

        throw new FileNotFoundException(
            $"Could not find ZulAi.Api project with appsettings.json. CWD: {cwd}");
    }
}
