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

    public static Task ShowAsync(string title, string content, ContentDialogButtons btns = ContentDialogButtons.YesNo, IFontIconInfo? fontIconInfo = null)
    { 
        return Current.ContentDialogService.ShowContentDialogAsync(title,content,btns, fontIconInfo ?? FontIcons.Filter("Info"));
    }

    public static Task ShowErrorAsync(string title, string content)
    {
        return ShowAsync(title, content, ContentDialogButtons.OK, FontIcons.Filter("Error"));
    }
}
