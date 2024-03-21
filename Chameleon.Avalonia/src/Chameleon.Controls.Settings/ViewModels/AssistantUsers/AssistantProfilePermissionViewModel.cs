using Chameleon.CT.Common.Base;
using Chameleon.Infrastructure.Users;
using Chameleon.Interfaces.App.Assistants;
using Chameleon.Interfaces.Dialogs;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels.AssistantUsers;

public partial class AssistantProfilePermissionViewModel
        : SubPageViewModelBase
{
    private readonly IUserAssistantService _userAssistantService;
    private readonly IToastNotificationService _toastNotificationService;

    public AssistantProfilePermissionViewModel(
        IUserAssistantService userAssistantService,
        IToastNotificationService toastNotificationService,
        IAssistantProfilePermission assistantProfilePermission
        )
    {
        _userAssistantService = userAssistantService;
        _toastNotificationService = toastNotificationService;

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

            _toastNotificationService.ShowSuccess($"{AssistantProfilePermission.DisplayName} was updated successfully");
        }
        catch
        {
            IsGranted = !IsGranted;

            _toastNotificationService.ShowError($"{AssistantProfilePermission.DisplayName} update failed. Please try again.");
        }
    }
}