using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskTracker.Data;

public sealed class TaskTrackerDbContextFactory : IDesignTimeDbContextFactory<TaskTrackerDbContext>
{
    public TaskTrackerDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("The 'DefaultConnection' connection string is missing.");

        var optionsBuilder = new DbContextOptionsBuilder<TaskTrackerDbContext>();
        optionsBuilder.UseSqlite(connectionString);

        return new TaskTrackerDbContext(optionsBuilder.Options);
    }
}
