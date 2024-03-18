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
public class DefaultContentDialogView(ContentDialogButtons btns, object content, object? title = null, string primaryBtnTxt = "OK", string secondaryBtnTxt = "", string closebtnTxt = "") :
    IDefaultContentDialogView
{
    public object? Title => title ?? ContainerServiceHelper.Current.ContainerProvider?.Resolve<IDefaultContentDialogTitle>();    
    public object? DialogContent => content;
    public string PrimaryButtonText => btns switch
    {
        ContentDialogButtons.OK or ContentDialogButtons.OKCancel => "OK",
        ContentDialogButtons.YesNoCancel or ContentDialogButtons.YesNo => "Yes",
        _ => primaryBtnTxt
    };
    public string SecondaryButtonText => btns switch
    {
        ContentDialogButtons.OK or 
        ContentDialogButtons.OKCancel or 
        ContentDialogButtons.YesNo => secondaryBtnTxt,
        ContentDialogButtons.YesNoCancel  => "No",
        _ => secondaryBtnTxt
    };
    public string CloseButtonText => btns switch
    {
        ContentDialogButtons.YesNoCancel or ContentDialogButtons.OKCancel => "Cancel",
        ContentDialogButtons.YesNo => "No",
        _ => closebtnTxt
    };
}
