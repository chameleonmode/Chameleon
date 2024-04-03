using Avalonia;
using Avalonia.Controls;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Services;

namespace Chameleon.Avalonia.Common.Services;

public class ClipboardService  : IClipboardService
{
    public static IClipboardService Instance { get; } = ContainerServiceHelper.Resolve<IClipboardService>() as ClipboardService;

    public  TopLevel? Owner { get; set; }

    public void SetOwner(object owner)
    {
        Owner = owner as TopLevel;
    }

    public Task SetTextAsync(string text)
    {
        Owner ??= TopLevel.GetTopLevel(ApplicationHelper.GetToplevetVisual());
        return Owner?.Clipboard.SetTextAsync(text);
    }
}
