namespace TaskTracker.Components.Pages;

public partial class ProfileSettings
{
    private async Task ChangePasswordAsync()
    {
        passwordMessage = null;
        isSavingPassword = true;

        var user = await GetCurrentUserAsync();

        if (user is null)
        {
            NavigationManager.NavigateTo("/Identity/Account/Login", forceLoad: true);
            return;
        }

        var result = await UserManager.ChangePasswordAsync(user, passwordModel.CurrentPassword, passwordModel.NewPassword);

        if (result.Succeeded)
        {
            await SignInManager.RefreshSignInAsync(user);
            hasPassword = await UserManager.HasPasswordAsync(user);
            ResetPasswordModel();
            passwordHasError = false;
            passwordMessage = "Password updated.";
            isEditingPassword = false;
        }
        else
        {
            passwordHasError = true;
            passwordMessage = string.Join(" ", result.Errors.Select(error => error.Description));
        }

        isSavingPassword = false;
    }

    private void BeginPasswordEdit()
    {
        isEditingPassword = true;
        passwordMessage = null;
        ResetPasswordModel();
    }

    private void CancelPasswordEdit()
    {
        isEditingPassword = false;
        passwordMessage = null;
        ResetPasswordModel();
    }

    private void ResetPasswordModel()
    {
        passwordModel.CurrentPassword = string.Empty;
        passwordModel.NewPassword = string.Empty;
        passwordModel.ConfirmPassword = string.Empty;
    }
}
