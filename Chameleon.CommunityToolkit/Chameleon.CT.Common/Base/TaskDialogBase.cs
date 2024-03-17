namespace Chameleon.CT.Common.Base;

public partial class TaskDialogBase : ObservableObjectBase
{
    [ObservableProperty]
    public string? header;

    [ObservableProperty]
    public string? subHeader;

    [ObservableProperty]
    public bool isInputEnabled = true;
}
