namespace Chameleon.Interfaces.Dialogs;

//
// Summary:
//     The result of the dialog.
public enum PopupDialogButtonResult
{
    //
    // Summary:
    //     Abort.
    Abort = 3,
    //
    // Summary:
    //     Cancel.
    Cancel = 2,
    //
    // Summary:
    //     Ignore.
    Ignore = 5,
    //
    // Summary:
    //     No.
    No = 7,
    //
    // Summary:
    //     No result returned.
    None = 0,
    //
    // Summary:
    //     OK.
    OK = 1,
    //
    // Summary:
    //     Retry.
    Retry = 4,
    //
    // Summary:
    //     Yes.
    Yes = 6,
    //
    // Summary:
    //     ???.
    Unset = 404
}

public interface IPopupDialogResult
{
    PopupDialogButtonResult ButtonResult { get; }
    public object ResultObject { get; set; }
}
