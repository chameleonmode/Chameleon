using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Prism.Events;

namespace Chameleon.Avalonia.Prism.Dialogs.ViewModels;

public class DialogWindowViewModel
    : DialogViewModelBase
    , IDialogWindowViewModel
{
    private readonly IEventAggregator _eventAggregator;

    public DialogWindowViewModel(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;

        _eventAggregator
            .GetEvent<CloseDialogWindowEvent>()
            .Subscribe(CloseDialog);
    }

    private string _title;
    public override string Title
    {
        get => _title;
        set
        {
            SetProperty(ref _title, value);
        }
    }
}
