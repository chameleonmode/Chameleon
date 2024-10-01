using Avalonia;
using Avalonia.Controls;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Services;
using Chameleon.lib.Common.Interfaces.Services;
using Chameleon.lib.Common.ServiceManagers;

namespace Chameleon.Avalonia.Common.Services;

public class ClipboardService  : IClipboardService
{
    private const string ClipboardText = "Copied to clipboard";

    private readonly IToastNotificationService _toastNotificationService = ContainerServiceHelper.Resolve<IToastNotificationService>();

    public static IClipboardService Instance { get; } = ContainerServiceHelper.Resolve<IClipboardService>() as ClipboardService;

    public  TopLevel? Owner { get; set; }

    public void SetOwner(object owner)
    {
        Owner = owner as TopLevel;
    }

    public async Task SetTextAsync(string text)
    {
        try
        {
            Owner ??= TopLevel.GetTopLevel(ApplicationHelper.GetToplevetVisual());
           await Owner?.Clipboard.SetTextAsync(text);
            _toastNotificationService.ShowSuccess(ClipboardText);

        }
        catch (Exception ex)
        {
            await MesageBoxHelper.ShowErrorAsync("Failed to copy to clipboard.", ex.Message);
        }
    }
}

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
