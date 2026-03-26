using Microsoft.EntityFrameworkCore;

namespace TaskTracker.Data;

public sealed class TaskTrackerDbContext(DbContextOptions<TaskTrackerDbContext> options) : DbContext(options)
{
}
