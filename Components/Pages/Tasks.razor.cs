using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;

namespace TaskTracker.Components.Pages;

[Authorize]
public partial class Tasks : ComponentBase
{
    private readonly List<TaskItem> tasks = [];
    private bool isLoading = true;
    private int? deleteCandidateId;
    private string? deleteCandidateTitle;
    private string? errorMessage;
    private string? userId;
    private string searchTerm = string.Empty;
    private TaskStatusFilter statusFilter = TaskStatusFilter.All;

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] private IDbContextFactory<TaskTrackerDbContext> DbContextFactory { get; set; } = default!;
    [Inject] private UserManager<ApplicationUser> UserManager { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private IEnumerable<TaskItem> FilteredTasks =>
        tasks.Where(task =>
            MatchesStatusFilter(task) &&
            (string.IsNullOrWhiteSpace(searchTerm) ||
             task.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
             (task.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)));

    protected override async Task OnInitializedAsync()
    {
        await EnsureUserIdAsync();
        await LoadTasksAsync();
    }

    private async Task EnsureUserIdAsync()
    {
        if (!string.IsNullOrEmpty(userId))
        {
            return;
        }

        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        userId = UserManager.GetUserId(authState.User);

        if (string.IsNullOrEmpty(userId))
        {
            NavigationManager.NavigateTo("/Identity/Account/Login", forceLoad: true);
        }
    }

    private async Task LoadTasksAsync()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            await EnsureUserIdAsync();

            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var results = await dbContext.Tasks
                .AsNoTracking()
                .Where(task => task.UserId == userId)
                .OrderBy(task => task.IsCompleted)
                .ThenBy(task => task.DueDateUtc ?? DateTime.MaxValue)
                .ThenByDescending(task => task.CreatedAtUtc)
                .ToListAsync();

            tasks.Clear();
            tasks.AddRange(results);
        }
        catch
        {
            errorMessage = "Tasks could not be loaded.";
        }
        finally
        {
            isLoading = false;
        }
    }

    private void PerformSearch() => StateHasChanged();

    private bool MatchesStatusFilter(TaskItem task) =>
        statusFilter switch
        {
            TaskStatusFilter.Pending => !task.IsCompleted,
            TaskStatusFilter.Completed => task.IsCompleted,
            _ => true
        };

    private enum TaskStatusFilter
    {
        All,
        Pending,
        Completed
    }
}
