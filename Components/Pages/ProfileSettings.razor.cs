using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using TaskTracker.Components.Profile;
using TaskTracker.Data;

namespace TaskTracker.Components.Pages;

[Authorize]
public partial class ProfileSettings : ComponentBase
{
    private readonly ProfileSettingsModel profileModel = new();
    private readonly ChangePasswordModel passwordModel = new();
    private bool isLoading = true;
    private bool isSavingProfile;
    private bool isSavingPassword;
    private bool isEditingProfile;
    private bool isEditingPassword;
    private bool hasPassword;
    private string? profileMessage;
    private bool profileHasError;
    private string? passwordMessage;
    private bool passwordHasError;
    private PendingChange pendingChange;

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] private UserManager<ApplicationUser> UserManager { get; set; } = default!;
    [Inject] private SignInManager<ApplicationUser> SignInManager { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private string ConfirmMessage => pendingChange switch
    {
        PendingChange.Profile => "Save these profile detail changes?",
        PendingChange.Password => "Change your password now?",
        _ => string.Empty
    };

    private string ProfileMessageClass => profileHasError ? "alert-danger" : "alert-success";
    private string PasswordMessageClass => passwordHasError ? "alert-danger" : "alert-success";

    protected override async Task OnInitializedAsync() => await LoadUserAsync();

    private async Task LoadUserAsync()
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            NavigationManager.NavigateTo("/Identity/Account/Login", forceLoad: true);
            return;
        }

        SyncProfileModel(user);
        hasPassword = await UserManager.HasPasswordAsync(user);
        isLoading = false;
    }

    private Task RequestProfileSaveAsync()
    {
        pendingChange = PendingChange.Profile;
        return Task.CompletedTask;
    }

    private Task RequestPasswordSaveAsync()
    {
        pendingChange = PendingChange.Password;
        return Task.CompletedTask;
    }

    private async Task ConfirmPendingChangeAsync()
    {
        if (pendingChange == PendingChange.Profile)
        {
            await SaveProfileAsync();
        }
        else if (pendingChange == PendingChange.Password)
        {
            await ChangePasswordAsync();
        }

        pendingChange = PendingChange.None;
    }

    private void ClearPendingChange() => pendingChange = PendingChange.None;

    private async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        return await UserManager.GetUserAsync(authState.User);
    }
}
