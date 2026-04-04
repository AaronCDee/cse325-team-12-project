using TaskTracker.Data;

namespace TaskTracker.Components.Pages;

public partial class ProfileSettings
{
    private async Task SaveProfileAsync()
    {
        profileMessage = null;
        isSavingProfile = true;

        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            NavigationManager.NavigateTo("/Identity/Account/Login", forceLoad: true);
            return;
        }

        user.FirstName = profileModel.FirstName.Trim();
        user.LastName = profileModel.LastName.Trim();

        var result = await UserManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            await SignInManager.RefreshSignInAsync(user);
            SyncProfileModel(user);
            profileHasError = false;
            profileMessage = "Profile details updated.";
            isEditingProfile = false;
        }
        else
        {
            profileHasError = true;
            profileMessage = string.Join(" ", result.Errors.Select(error => error.Description));
        }

        isSavingProfile = false;
    }

    private void BeginProfileEdit()
    {
        isEditingProfile = true;
        profileMessage = null;
    }

    private async Task CancelProfileEdit()
    {
        var user = await GetCurrentUserAsync();

        if (user is not null)
        {
            SyncProfileModel(user);
        }

        isEditingProfile = false;
        profileMessage = null;
    }

    private void SyncProfileModel(ApplicationUser user)
    {
        profileModel.FirstName = user.FirstName ?? string.Empty;
        profileModel.LastName = user.LastName ?? string.Empty;
        profileModel.Email = user.Email ?? string.Empty;
    }
}
