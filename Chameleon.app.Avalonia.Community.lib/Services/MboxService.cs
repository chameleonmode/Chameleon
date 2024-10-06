using chameleon.assets;

using Chameleon.app.Avalonia.Community.lib.ViewModels;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Interfaces.Services;

using FluentAvalonia.UI.Controls;

namespace Chameleon.app.Avalonia.Community.lib.Services;
public class MboxService(IDispatchService dispatcher) : IMboxService {
	public async Task<Enums.MboxResult> ShowAsync(string title, string content, Enums.MBoxButtons btns = Enums.MBoxButtons.YesNo, string icon = "Info")
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
}

