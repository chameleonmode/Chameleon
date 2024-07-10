using Chameleon.Common.Icons;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Services;

namespace Chameleon.Common.Helpers;

public class MesageBoxHelper
{
    readonly IContentDialogService ContentDialogService;
    readonly IDispatcherService DispatcherService;

    private MesageBoxHelper()
    {
        ContentDialogService = ContainerServiceHelper.Resolve<IContentDialogService>();
        DispatcherService = ContainerServiceHelper.Resolve<IDispatcherService>();
    }
    public static MesageBoxHelper Current { get; } = new MesageBoxHelper();

    public static async Task<bool> ShowAsync(string title, string content, ContentDialogButtons btns = ContentDialogButtons.YesNo, string? fontIconInfo = null, IContentDialogResult retVal = IContentDialogResult.Primary)
    {
        bool isOnUIThread = SynchronizationContext.Current != null && SynchronizationContext.Current.GetType() != typeof(SynchronizationContext);
        if (!isOnUIThread)
        {
            return await Current.DispatcherService.InvokeOnUiThread(async () => 
            {
                return await Current.ContentDialogService.ShowContentDialogAsync(title, content, btns, FontIcons.Filter(fontIconInfo ?? "Info")) == retVal;
            });
        }
        return await Current.ContentDialogService.ShowContentDialogAsync(title,content,btns, FontIcons.Filter(fontIconInfo ?? "Info")) == retVal;
    }

    public static Task<bool> ShowErrorAsync(string title, string content)
    {
        return ShowAsync(title, content, ContentDialogButtons.OK, "Error");
    }
}
