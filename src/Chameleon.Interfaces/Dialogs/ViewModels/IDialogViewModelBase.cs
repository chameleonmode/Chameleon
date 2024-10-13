namespace Chameleon.Interfaces.Dialogs.ViewModels;

public interface IDialogViewModelBase
{
    Task<IContentDialogResult> ShowAsync();
}
