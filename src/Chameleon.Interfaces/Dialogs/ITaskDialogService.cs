namespace Chameleon.Interfaces.Dialogs;

public interface ITaskDialogService
{
    Task ShowTaskDialog(Type content, Action action); 
    Task<ITaskDialogResult?> ShowTaskDialog(Type content);
}
