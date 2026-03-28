using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TaskTracker.Data;

public sealed class TaskTrackerDbContext(DbContextOptions<TaskTrackerDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
}
