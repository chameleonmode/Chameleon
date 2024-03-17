namespace Chameleon.Interfaces.Dialogs;

public interface IContentDialogService
{
    Task<IContentDialogResult> ShowContentDialogAsync(Type contentDialog);
}