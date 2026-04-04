using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;

namespace TaskTracker.Components.Pages;

public partial class Tasks
{
    private void SetDeleteCandidate(TaskItem task)
    {
        deleteCandidateId = task.Id;
        deleteCandidateTitle = task.Title;
    }

    private void ClearDeleteCandidate()
    {
        deleteCandidateId = null;
        deleteCandidateTitle = null;
    }

    private async Task ToggleCompletionAsync(int taskId)
    {
        await EnsureUserIdAsync();

        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        await using var dbContext = await DbContextFactory.CreateDbContextAsync();
        var task = await dbContext.Tasks.SingleOrDefaultAsync(item => item.Id == taskId && item.UserId == userId);

        if (task is null)
        {
            errorMessage = "That task could not be found.";
            return;
        }

        task.IsCompleted = !task.IsCompleted;
        await dbContext.SaveChangesAsync();
        await LoadTasksAsync();
    }

    private async Task ConfirmDeleteAsync()
    {
        if (deleteCandidateId is null)
        {
            return;
        }

        await EnsureUserIdAsync();

        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        await using var dbContext = await DbContextFactory.CreateDbContextAsync();
        var task = await dbContext.Tasks.SingleOrDefaultAsync(item => item.Id == deleteCandidateId && item.UserId == userId);

        if (task is null)
        {
            errorMessage = "That task could not be found.";
            ClearDeleteCandidate();
            return;
        }

        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync();
        ClearDeleteCandidate();
        await LoadTasksAsync();
    }
}
