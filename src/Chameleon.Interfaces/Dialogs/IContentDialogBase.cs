namespace Chameleon.Interfaces.Dialogs;

public interface IViewModelAware 
{
  
}
public interface IViewAware
{
    T? GetView<T>();
}
public interface IContentDialogView :
    IViewModelAware,
    IContentDialogAware
{
}
public interface IContentDialogAware
{
    ContentDialogButtons DialogButtons { get; set; }
    object? Title { get; set; }
    object? DialogContent { get; set; }
    string? Glyph { get; set; }
    string PrimaryButtonText { get; }
    string SecondaryButtonText { get; }
    string CloseButtonText { get; }
}
