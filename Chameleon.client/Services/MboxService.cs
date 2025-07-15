using FluentAvalonia.UI.Controls;

using chameleon.assets;

using Chameleon.client.UI.Components;

using CommunityToolkit.Mvvm.ComponentModel;
using Chameleon.lib.Helpers;
using Chameleon.lib;
using Chameleon.lib.Services;

namespace Chameleon.client.Services;

public partial class MBoxViewModel : ObservableObject {
	[ObservableProperty] string title = IoC.AppName;
	[ObservableProperty] string glyph = "E946";
}
public class MboxService(IDispatchService dispatcher) : IMboxService {
	public async Task<MboxResult> Show(string title, string content, MBoxButtons btns = MBoxButtons.YesNo, string icon = "Info") {
		var c = new MBoxViewModel();
		var icons = await Icons.Instance.FontIcons;
		c.Title = title;
		c.Glyph = icons.FirstOrDefault(i => i.Name == icon)?.Glyph ?? "E946";

		return await dispatcher.InvokeOnUiThread(async () => {
			var dialog = new ContentDialog() {
				Title = new MboxTitleUserControl(),
				Content = content,
				DataContext = c,
				PrimaryButtonText = btns switch {
					MBoxButtons.Ok or MBoxButtons.OkCancel => "OK",
					MBoxButtons.YesNoCancel or MBoxButtons.YesNo => "Yes",
					_ => "OK"
				},
				SecondaryButtonText = btns switch {
					MBoxButtons.YesNoCancel => "No",
					_ => null
				},
				CloseButtonText = btns switch {
					MBoxButtons.YesNo => "No",
					MBoxButtons.YesNoCancel or
					MBoxButtons.OkCancel => "Cancel",
					_ => null
				},
				DefaultButton = ContentDialogButton.Primary,
			};
			var res = await dialog.ShowAsync();
			return (MboxResult)res;
		});
	}
	public async Task<TaskDialogResult> ShowTaskDialog<TViewModel, Tview>(Func<TViewModel> initialize, string header, string? subHeader = null, string title = IoC.AppName, object? footer = null, Symbas symbas = Symbas.Alert, MBoxButtons btns = MBoxButtons.YesNo) where Tview : new() {
		var result = await ShowTaskDialog(
		initialize,
		dispatcher.InvokeOnUiThread(() => Task.FromResult(new Tview())),
		header,
		subHeader,
		title,
		footer,
		symbas,
		btns);
		return result;
	}
	public async Task<TaskDialogResult> ShowTaskDialog<TViewModel>(Func<TViewModel> initialize, object content, string header, string? subHeader = null, string title = IoC.AppName, object? footer = null, Symbas symbas = Symbas.Alert, MBoxButtons btns = MBoxButtons.YesNo) {
		while (App.MainWindow is null) {
			await Task.Delay(250);
		}
		return await dispatcher.InvokeOnUiThread(async () => {
			var btnsList = new List<TaskDialogButton>();
			switch (btns) {
				case MBoxButtons.YesNo:
					btnsList.Add(TaskDialogButton.YesButton);
					btnsList.Add(TaskDialogButton.NoButton);
					break;
				case MBoxButtons.YesNoCancel:
					btnsList.Add(TaskDialogButton.YesButton);
					btnsList.Add(TaskDialogButton.NoButton);
					btnsList.Add(TaskDialogButton.CancelButton);
					break;
				case MBoxButtons.Ok:
					btnsList.Add(TaskDialogButton.OKButton);
					break;
				case MBoxButtons.OkCancel:
					btnsList.Add(TaskDialogButton.OKButton);
					btnsList.Add(TaskDialogButton.CancelButton);
					break;
				case MBoxButtons.RetryCancel:
					btnsList.Add(TaskDialogButton.RetryButton);
					btnsList.Add(TaskDialogButton.CancelButton);
					break;
				case MBoxButtons.Close:
					btnsList.Add(TaskDialogButton.CloseButton);
					break;
			}
			// Declaring a TaskDialog from C#:
			var td = new TaskDialog {
				// Title property only applies on Windowed dialogs
				Title = title,
				Header = header,
				SubHeader = subHeader,
				Content = content,
				IconSource = new SymbolIconSource { Symbol = (Symbol)(int)symbas },
				FooterVisibility = footer == null ? TaskDialogFooterVisibility.Never : TaskDialogFooterVisibility.Auto,
				Footer = footer,
				DataContext = initialize(),
				//	Commands =
				//		{
				//		new TaskDialogCommand
				//		{
				//				Text = "Command Text",
				//				Description = "Description",
				//				DialogResult = "CommandResult",
				//          // ClosesOnInvoked property lets you choose if invoking this command closes the dialog
				//          // automatically (default true)
				//          ClosesOnInvoked = true,
				//          // Can also set IconSource
				//          // Can also set IsEnabled
				//      }
				//},
				Buttons = btnsList,
				//{
				//     // For more advanced scenarios, you can create your own buttons
				//     // Custom buttons allow you to attach icons, custom results,
				//     // command and click handlers to fully customize your experience
				//     // Note: 'null' is not a valid dialog result and is automatically
				//     // converted to TaskDialogStandardResult.None when the dialog closes
				//     //new TaskDialogButton("OK" /* text */, "myResult" /* dialogResult */)

				//     // There are some default buttons for simple cases provided
				//     // These have predefinded text and results that correspond to
				//     // TaskDialogStandardResult enum
				//     // Note that built in buttons cannot have Commands, Icons, or
				//     // click handlers attached to them
				//     TaskDialogButton.OKButton,
				//	TaskDialogButton.CancelButton,
				//	TaskDialogButton.YesButton,
				//	TaskDialogButton.NoButton,
				//	TaskDialogButton.RetryButton,
				//	TaskDialogButton.CloseButton
				//},
				// Before showing a dialog declared in C#, you MUST set the XamlRoot property
				// Using the VisualRoot is fine, if the VisualRoot is a Window, the dialog automatically launches in
				// Windowed mode, otherwise, it tries to find the OverlayLayer and will launch in hosted mode
				// If your TaskDialog is declared in Xaml, this is automatically handled for you
				XamlRoot = App.MainWindow
			};

			var result = await td.ShowAsync(true);
			return result switch {
				"myResult" => TaskDialogResult.OK,
				TaskDialogStandardResult.Cancel => TaskDialogResult.Cancel,
				TaskDialogStandardResult.Yes => TaskDialogResult.Yes,
				TaskDialogStandardResult.No => TaskDialogResult.No,
				TaskDialogStandardResult.Retry => TaskDialogResult.Retry,
				TaskDialogStandardResult.Close => TaskDialogResult.Close,
				TaskDialogStandardResult.OK => TaskDialogResult.OK,
				_ => TaskDialogResult.None,
			};
		});

		// If you want to force hosted mode, ShowAsync accepts a parameter 'showHosted' to force this mode
		//var result = await td.ShowAsync(true);
	}
}