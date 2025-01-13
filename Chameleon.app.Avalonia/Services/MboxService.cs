using FluentAvalonia.UI.Controls;

using chameleon.assets;

using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Interfaces.Services;
using Chameleon.app.Avalonia.lib.Community.Controls;
using Avalonia.Controls;
using Chameleon.app.Avalonia.ViewModels.Controllers;
using Chameleon.lib.Common;
using Chameleon.lib.CommunityToolkit.MvvM;

namespace Chameleon.app.Avalonia.Services;
public class MboxService(IDispatchService dispatcher) : IMboxService {
	public async Task<Enums.MboxResult> Show(string title, string content, Enums.MBoxButtons btns = Enums.MBoxButtons.YesNo, string icon = "Info")
	{
		var c = new MBoxViewModel();
		var icons = await Icons.Instance.FontIcons;
		c.Title = title;
		c.Glyph = icons.FirstOrDefault(i => i.Name == icon)?.Glyph ?? "E946";

		return await dispatcher.InvokeOnUiThread(async () => {

			var dialog = new ContentDialog() {
				Title = new MboxTitleUserControl(),
				Content = content,
				DataContext = c,
				PrimaryButtonText = btns.PrimaryBtnText(),
				SecondaryButtonText = btns.SecondaryBtnText(),
				CloseButtonText = btns.CloseBtnText(),
				DefaultButton = ContentDialogButton.Primary,
			};
			var res = await dialog.ShowAsync();
			return (Enums.MboxResult)res;
		});
	}

	public async Task<Enums.MboxResult> ShowContentDialog<TView, TViewModel>(Action<TViewModel> initialize)
	{
		if (IoC.GetService<TView>() is Control view) {
			var viewModel = (TViewModel)view.DataContext!;

			initialize?.Invoke(viewModel);

			var title = viewModel is ViewModelObjectBase pvm ? pvm.Title : Consts.AppName;

			var btns = Enums.MBoxButtons.OkCancel;

			var dialog = new ContentDialog() {
				Title = title,
				Content = view,
				PrimaryButtonText = btns.PrimaryBtnText(),
				SecondaryButtonText = btns.SecondaryBtnText(),
				CloseButtonText = btns.CloseBtnText(),
				DefaultButton = ContentDialogButton.Primary,
			};

			//if (viewModel is IContentDialogViewModel cdvm) {
			//	dialog.Closing += (s, e) => {
			//		cdvm.OnDialogClosing((IContentDialogResult)e.Result);
			//	};
			//}
			var res = await dialog.ShowAsync(AppLayers.GetMainWindow());
			return (Enums.MboxResult)res;
		}

		throw new ArgumentNullException("TView");
	}

	public async Task<Enums.TaskDialogResult> ShowTaskDialog<TViewModel>(Func<TViewModel> initialize, object content, string header, string? subHeader = null, string title = Consts.AppName, object? footer = null, Enums.Symbas symbas = Enums.Symbas.Alert, Enums.MBoxButtons btns = Enums.MBoxButtons.YesNo)
	{
		return await dispatcher.InvokeOnUiThread(async () => {
			var btnsList = new List<TaskDialogButton>();
			switch (btns) {
				case Enums.MBoxButtons.YesNo:
					btnsList.Add(TaskDialogButton.YesButton);
					btnsList.Add(TaskDialogButton.NoButton);
					break;
				case Enums.MBoxButtons.YesNoCancel:
					btnsList.Add(TaskDialogButton.YesButton);
					btnsList.Add(TaskDialogButton.NoButton);
					btnsList.Add(TaskDialogButton.CancelButton);
					break;
				case Enums.MBoxButtons.Ok:
					btnsList.Add(TaskDialogButton.OKButton);
					break;
				case Enums.MBoxButtons.OkCancel:
					btnsList.Add(TaskDialogButton.OKButton);
					btnsList.Add(TaskDialogButton.CancelButton);
					break;
				case Enums.MBoxButtons.RetryCancel:
					btnsList.Add(TaskDialogButton.RetryButton);
					btnsList.Add(TaskDialogButton.CancelButton);
					break;
				case Enums.MBoxButtons.Close:
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
				XamlRoot = AppLayers.GetMainWindow()
			};

			var result = await td.ShowAsync(true);
			return result switch {
				"myResult" => Enums.TaskDialogResult.OK,
				TaskDialogStandardResult.Cancel => Enums.TaskDialogResult.Cancel,
				TaskDialogStandardResult.Yes => Enums.TaskDialogResult.Yes,
				TaskDialogStandardResult.No => Enums.TaskDialogResult.No,
				TaskDialogStandardResult.Retry => Enums.TaskDialogResult.Retry,
				TaskDialogStandardResult.Close => Enums.TaskDialogResult.Close,
				TaskDialogStandardResult.OK => Enums.TaskDialogResult.OK,
				_ => Enums.TaskDialogResult.None,
			};
		});

		// If you want to force hosted mode, ShowAsync accepts a parameter 'showHosted' to force this mode
		//var result = await td.ShowAsync(true);
	}
}