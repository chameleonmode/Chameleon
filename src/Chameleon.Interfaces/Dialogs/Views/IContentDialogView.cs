namespace Chameleon.Interfaces.Dialogs.Views;

public interface IContentDialogView : IDefaultContentDialogView
{
    T?  GetDataContext<T>();
}
public interface IDefaultContentDialogView
{
    object? Title { get; }
    object? DialogContent { get; }
    string PrimaryButtonText { get; }
    string SecondaryButtonText { get; }
    string CloseButtonText { get; }
}
