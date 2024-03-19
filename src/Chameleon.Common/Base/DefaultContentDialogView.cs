using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Dialogs.Views;

namespace Chameleon.Common.Base;

/// <summary>
/// use for stuff like simple message box
/// </summary>
/// <param name="primaryBtnTxt"></param>
/// <param name="closebtnTxt"></param>
/// <param name="content"></param>
/// <param name="secondaryBtnTxt"></param>
/// <param name="title"></param>
public class DefaultContentDialogView(ContentDialogButtons btns, object content, object? title = null, string? primaryBtnTxt = null, string? secondaryBtnTxt = null, string? closebtnTxt = null) :
    IDefaultContentDialogView
{
    public object? Title => title ?? ContainerServiceHelper.Current.ContainerProvider?.Resolve<IDefaultContentDialogTitle>();
    public object? DialogContent => content;
    public string PrimaryButtonText => primaryBtnTxt ?? btns switch
    {
        ContentDialogButtons.OK or ContentDialogButtons.OKCancel => "OK",
        ContentDialogButtons.YesNoCancel or ContentDialogButtons.YesNo => "Yes",
        _ => "OK"
    };
    public string SecondaryButtonText => secondaryBtnTxt ?? btns switch
    {
        ContentDialogButtons.YesNoCancel => "No",
        ContentDialogButtons.OK or
        ContentDialogButtons.OKCancel or
        ContentDialogButtons.YesNo or
         _ => "No"
    };
    public string CloseButtonText => closebtnTxt ?? btns switch
    {
        ContentDialogButtons.YesNo => "No",
        ContentDialogButtons.YesNoCancel or
        ContentDialogButtons.OKCancel or
        _ => "Cancel"
    };
}
