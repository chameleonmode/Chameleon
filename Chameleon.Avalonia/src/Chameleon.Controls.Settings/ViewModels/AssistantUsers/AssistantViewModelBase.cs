using Chameleon.Domain.Entities.Assistants;
using Chameleon.Interfaces.App.Assistants.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public partial class AssistantViewModelBase : ObservableObject
{
    public virtual string Name => "AssistantViewModelBase";

    [RelayCommand]
    public virtual void Unshare()
    {
        OnPropertyChanged(string.Empty);
    }
}
