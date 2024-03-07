namespace Chameleon.Interfaces.Dialogs;

public interface IDialog
{
    string Title { get; set; }
    int Result { get; }
    object Content { get; set; }
    void Show();
    Task ShowDialog();
    object GetDialogViewModel();
}
