using Avalonia.Controls;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.lib.Common.Interfaces.Services;
using Chameleon.lib.Common.ServiceManagers;

namespace Chameleon.app.Avalonia.Services;
//ICopyPastaService
public class CopyPastaService : ICopyPastaService {
	public TopLevel? Owner { get; set; }
	public async Task SetTextAsync(string text)
	{
		try {
			Owner ??= TopLevel.GetTopLevel(ApplicationHelper.GetToplevetVisual());
			await Owner!.Clipboard!.SetTextAsync(text);
			Toaster.ShowSuccess("Copied to clipboard");

		} catch (Exception ex) {
			Toaster.ShowErr(ex.Message);
		}
	}
}
