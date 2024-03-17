namespace Chameleon.Interfaces.Dialogs.Views;

public interface IContentDialogView
{
    object? Title { get; }
    string PrimaryButtonText { get;  }
    string SecondaryButtonText { get;  }
    string CloseButtonText { get; }
}
