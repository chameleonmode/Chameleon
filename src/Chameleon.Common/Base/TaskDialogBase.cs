using Chameleon.Interfaces.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.Common.Base;

public partial class TaskDialogBase : CTViewModelBase
{
    public event Action<TaskDialogResul>? RequestClose;

    [ObservableProperty]
    public string? header;

    [ObservableProperty]
    public string? subHeader;       

    [ObservableProperty]
    public bool isInputEnabled = true;

    public virtual void Close(TaskDialogResul res = TaskDialogResul.None)
    {
        RequestClose?.Invoke(res);
    }
}
