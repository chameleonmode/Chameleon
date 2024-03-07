using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Interfaces.App.UserSettings;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Prism.Events;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels;

public class BulkAddPagesPopupViewModel
       : ViewModelBase
       , IBulkAddPagesPopupViewModel
{
    private readonly IEventAggregator _eventAggregator;

    public BulkAddPagesPopupViewModel(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;

        DiscardCommand = new DelegateCommand(Discard);
        SaveChangesCommand = new DelegateCommand(SaveChanges);
    }

    private string _urls;
    public string Urls
    {
        get => _urls;
        set => SetProperty(ref _urls, value);
    }

    private void Close()
    {
        _eventAggregator
            .GetEvent<CloseDialogWindowEvent>()
            .Publish((int)ButtonResult.Cancel);
    }

    public DelegateCommand DiscardCommand { get; }
    private void Discard()
    {
        Urls = null;
        Close();
    }

    public DelegateCommand SaveChangesCommand { get; }
    private void SaveChanges()
    {
        _eventAggregator
            .GetEvent<CloseDialogWindowEvent>()
            .Publish((int)ButtonResult.OK);
    }
}
