using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Chameleon.lib.Helpers;
using Chameleon.lib.Interfaces.Services;

namespace Chameleon.client.Services;
//ICopyPastaService
public class CopyPastaService : ICopyPastaService {
	public TopLevel? Owner { get; set; }
	public async Task SetTextAsync(string text) {
		try {
			Owner ??= TopLevel.GetTopLevel(App.MainWindow?.GetVisualRoot() as Visual);
			await Owner!.Clipboard!.SetTextAsync(text);
			Toaster.Success("Copied to clipboard");

		} catch (Exception ex) {
			Toaster.Error(ex.Message);
		}
	}
}
