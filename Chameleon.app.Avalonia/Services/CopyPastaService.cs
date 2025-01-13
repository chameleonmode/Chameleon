using Avalonia.Controls;

using Chameleon.lib.Common.Interfaces.Services;
using Chameleon.lib.Common.ServiceManagers;

namespace Chameleon.app.Avalonia.Services;
//ICopyPastaService
public class CopyPastaService : ICopyPastaService {
	public TopLevel? Owner { get; set; }
	public async Task SetTextAsync(string text)
	{
		try {
			Owner ??= TopLevel.GetTopLevel(AppLayers.GetToplevetVisual());
			await Owner!.Clipboard!.SetTextAsync(text);
			Toaster.Success("Copied to clipboard");

		} catch (Exception ex) {
			Toaster.Error(ex.Message);
		}
	}
}
