namespace Chameleon.Interfaces.Dialogs;
 public enum TaskDialogResul
{
    None,
    OK,
    Cancel,
    Yes,
    No,
    Retry,
    Close
}
public interface ITaskDialogService
{
    Task ShowTaskDialog(Type content, Action action); 
    Task<object?> ShowTaskDialog(Type content);
}
