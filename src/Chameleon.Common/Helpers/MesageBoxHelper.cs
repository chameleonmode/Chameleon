using Chameleon.Common.Icons;
using Chameleon.Interfaces.Dialogs;

namespace Chameleon.Common.Helpers;

public class MesageBoxHelper
{
    readonly IContentDialogService ContentDialogService;

    private MesageBoxHelper()
    {
        ContentDialogService = ContainerServiceHelper.Resolve<IContentDialogService>();
    }
    public static MesageBoxHelper Current { get; } = new MesageBoxHelper();

    public static async Task<bool> ShowAsync(string title, string content, ContentDialogButtons btns = ContentDialogButtons.YesNo, string? fontIconInfo = null, IContentDialogResult retVal = IContentDialogResult.Primary)
    { 
        return await Current.ContentDialogService.ShowContentDialogAsync(title,content,btns, FontIcons.Filter(fontIconInfo ?? "Info")) == retVal;
    }

    public static Task<bool> ShowErrorAsync(string title, string content)
    {
        return ShowAsync(title, content, ContentDialogButtons.OK, "Error");
    }
}
