using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TaskTracker.Data;

public sealed class TaskTrackerDbContext(DbContextOptions<TaskTrackerDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Tasks");
            entity.Property(task => task.CreatedAtUtc)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(task => task.UserId);
            entity.HasOne(task => task.User)
                .WithMany(user => user.Tasks)
                .HasForeignKey(task => task.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
