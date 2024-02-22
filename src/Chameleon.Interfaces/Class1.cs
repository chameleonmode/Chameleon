using Chameleon;
using Chameleon.Interfaces.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Chameleon.Prism.Services.Dialogs
{
    //
    // Summary:
    //     Interface for a dialog hosting window.
    public interface IDialogWindow
    {
        //
        // Summary:
        //     Dialog content.
        object Content { get; set; }

        //
        // Summary:
        //     The window's owner.
        //Window Owner { get; set; }

        //
        // Summary:
        //     The data context of the window.
        //
        // Remarks:
        //     The data context must implement Prism.Services.Dialogs.IDialogAware.
        object DataContext { get; set; }

        //
        // Summary:
        //     The result of the dialog.
        IDialogResult Result { get; set; }

        //
        // Summary:
        //     The window style.
        //Style Style { get; set; }

        //
        // Summary:
        //     Called when the window is loaded.
        //event RoutedEventHandler Loaded;

        //
        // Summary:
        //     Called when the window is closed.
        event EventHandler Closed;

        //
        // Summary:
        //     Called when the window is closing.
        event CancelEventHandler Closing;

        //
        // Summary:
        //     Close the window.
        void Close();

        //
        // Summary:
        //     Show a non-modal dialog.
        void Show();

        //
        // Summary:
        //     Show a modal dialog.
        bool? ShowDialog();
    }

    //
    // Summary:
    //     Contains Prism.Services.Dialogs.IDialogParameters from the dialog and the Prism.Services.Dialogs.ButtonResult
    //     of the dialog.
    public interface IDialogResult
    {
        //
        // Summary:
        //     The parameters from the dialog.
        IDialogParameters Parameters { get; }

        //
        // Summary:
        //     The result of the dialog.
        ButtonResult Result { get; }
    }

    //
    // Summary:
    //     Represents Dialog parameters.
    //
    // Remarks:
    //     A class that implements this interface can be used to pass object parameters
    //     during the showing and closing of Dialogs.
    public interface IDialogParameters
    {
        //
        // Summary:
        //     The number of parameters in the collection.
        int Count { get; }

        //
        // Summary:
        //     The keys in the collection.
        IEnumerable<string> Keys { get; }

        //
        // Summary:
        //     Adds the key and value to the collection.
        //
        // Parameters:
        //   key:
        //     The key to reference this parameter value in the collection.
        //
        //   value:
        //     The parameter value to store.
        void Add(string key, object value);

        //
        // Summary:
        //     Checks the collection for the presence of a key.
        //
        // Parameters:
        //   key:
        //     The key to check.
        //
        // Returns:
        //     true if key exists; false otherwise.
        bool ContainsKey(string key);

        //
        // Summary:
        //     Gets the parameter value referenced by a key.
        //
        // Parameters:
        //   key:
        //     The key of the parameter value to be returned.
        //
        // Type parameters:
        //   T:
        //     The type of object to be returned.
        //
        // Returns:
        //     The matching parameter of type T.
        T GetValue<T>(string key);

        //
        // Summary:
        //     Gets all parameter values referenced by a key.
        //
        // Parameters:
        //   key:
        //     The key of the parameter values to be returned.
        //
        // Type parameters:
        //   T:
        //     The type of object to be returned.
        //
        // Returns:
        //     All matching parameter values of type T.
        IEnumerable<T> GetValues<T>(string key);

        //
        // Summary:
        //     Gets the parameter value if the referenced key exists.
        //
        // Parameters:
        //   key:
        //     The key of the parameter value to be returned.
        //
        //   value:
        //     The matching parameter of type T if the key exists.
        //
        // Type parameters:
        //   T:
        //     The type of object to be returned.
        //
        // Returns:
        //     true if the parameter exists; false otherwise.
        bool TryGetValue<T>(string key, out T value);
    }

    //
    // Summary:
    //     Interface to show modal and non-modal dialogs.
    public interface IDialogService
    {
        //
        // Summary:
        //     Shows a non-modal dialog.
        //
        // Parameters:
        //   name:
        //     The name of the dialog to show.
        //
        //   parameters:
        //     The parameters to pass to the dialog.
        //
        //   callback:
        //     The action to perform when the dialog is closed.
        void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback);

        //
        // Summary:
        //     Shows a non-modal dialog.
        //
        // Parameters:
        //   name:
        //     The name of the dialog to show.
        //
        //   parameters:
        //     The parameters to pass to the dialog.
        //
        //   callback:
        //     The action to perform when the dialog is closed.
        //
        //   windowName:
        //     The name of the hosting window registered with the IContainerRegistry.
        void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName);

        //
        // Summary:
        //     Shows a modal dialog.
        //
        // Parameters:
        //   name:
        //     The name of the dialog to show.
        //
        //   parameters:
        //     The parameters to pass to the dialog.
        //
        //   callback:
        //     The action to perform when the dialog is closed.
        void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback);

        //
        // Summary:
        //     Shows a modal dialog.
        //
        // Parameters:
        //   name:
        //     The name of the dialog to show.
        //
        //   parameters:
        //     The parameters to pass to the dialog.
        //
        //   callback:
        //     The action to perform when the dialog is closed.
        //
        //   windowName:
        //     The name of the hosting window registered with the IContainerRegistry.
        void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName);
    }

    //
    // Summary:
    //     Interface that provides dialog functions and events to ViewModels.
    public interface IDialogAware
    {
        //
        // Summary:
        //     The title of the dialog that will show in the window title bar.
        string Title { get; }

        //
        // Summary:
        //     Instructs the Prism.Services.Dialogs.IDialogWindow to close the dialog.
        event Action<IDialogResult> RequestClose;

        //
        // Summary:
        //     Determines if the dialog can be closed.
        //
        // Returns:
        //     If true the dialog can be closed. If false the dialog will not close.
        bool CanCloseDialog();

        //
        // Summary:
        //     Called when the dialog is closed.
        void OnDialogClosed();

        //
        // Summary:
        //     Called when the dialog is opened.
        //
        // Parameters:
        //   parameters:
        //     The parameters passed to the dialog.
        void OnDialogOpened(IDialogParameters parameters);
    }
}
