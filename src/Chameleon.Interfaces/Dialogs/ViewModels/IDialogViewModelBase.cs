namespace Chameleon.Interfaces.Dialogs.ViewModels;

public interface IDialogViewModelBase
{
    IContentDialogService ContentDialogService { get; }
    Task<IContentDialogResult> ShowAsync();
}
