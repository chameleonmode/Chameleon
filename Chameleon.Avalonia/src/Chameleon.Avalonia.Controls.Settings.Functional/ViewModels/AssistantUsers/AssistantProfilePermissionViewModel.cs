using Chameleon.CT.Common.Base;
using Chameleon.Infrastructure.Users;
using Chameleon.Interfaces.App.Assistants;
using Chameleon.Interfaces.Dialogs;
using Chameleon.lib.Common.ServiceManagers;

using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels.AssistantUsers;

public partial class AssistantProfilePermissionViewModel
        : SubPageViewModelBase
{
    private readonly IUserAssistantService _userAssistantService;

    public AssistantProfilePermissionViewModel(
        IUserAssistantService userAssistantService,
        IAssistantProfilePermission assistantProfilePermission
        )
    {
        _userAssistantService = userAssistantService;

        AssistantProfilePermission = assistantProfilePermission;
    }

    public IAssistantProfilePermission AssistantProfilePermission { get; }
    public string PermissionName => AssistantProfilePermission.PermissionName;
    public bool IsGranted
    {
        get => AssistantProfilePermission.IsGranted;
        set
        {
            if (AssistantProfilePermission.IsGranted != value)
            {
                AssistantProfilePermission.IsGranted = value;
                OnPropertyChanged(nameof(IsGranted));
            }
        }
    }

    [RelayCommand]
    private void UpdatePermission()
    {
        try
        {
            _userAssistantService.UpdateProfilePermission(AssistantProfilePermission);

            Toaster.ShowSuccess($"{AssistantProfilePermission.DisplayName} was updated successfully");
        }
        catch
        {
            IsGranted = !IsGranted;

			Toaster.ShowErr($"{AssistantProfilePermission.DisplayName} update failed. Please try again.");
        }
    }
}