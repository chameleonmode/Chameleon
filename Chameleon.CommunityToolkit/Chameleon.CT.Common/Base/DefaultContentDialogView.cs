using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Dialogs.Views;

namespace Chameleon.CT.Common.Base;
 public abstract partial class ContentDialogAwareBase : ObservableObjectBase,
    IContentDialogAware
{
    readonly string? primaryBtnTxt = null;
    readonly string? secondaryBtnTxt = null;
    readonly string? closebtnTxt = null;
    [ObservableProperty]
    new object? title = "ContentDialogAwareBase ttl";
    [ObservableProperty]
    object? dialogContent = "ContentDialogAwareBase";

    public ContentDialogAwareBase(ContentDialogButtons btns = ContentDialogButtons.YesNo, 
        string? primaryBtnTxt = null,
        string? secondaryBtnTxt = null,
        string? closebtnTxt = null)
    {
        DialogButtons = btns;
        this.primaryBtnTxt = primaryBtnTxt;
        this.secondaryBtnTxt = secondaryBtnTxt;
        this.closebtnTxt = closebtnTxt;
    }
    //public virtual new object? Title { get => title; }// => "ContentDialogAwareBase Title";
    //public virtual object? DialogContent => "ContentDialogAwareBase Content";

    public string PrimaryButtonText => primaryBtnTxt ?? GetPrimaryButtonText(DialogButtons);
    public string SecondaryButtonText => secondaryBtnTxt ?? GetSecondaryButtonText(DialogButtons);
    public string CloseButtonText => closebtnTxt ?? GetCloseButtonText(DialogButtons);

    public static string GetPrimaryButtonText(ContentDialogButtons btns) => btns switch
    {
        ContentDialogButtons.OK or ContentDialogButtons.OKCancel => "OK",
        ContentDialogButtons.YesNoCancel or ContentDialogButtons.YesNo => "Yes",
        _ => "OK"
    };

    public static string GetSecondaryButtonText(ContentDialogButtons btns) => btns switch
    {
        ContentDialogButtons.YesNoCancel => "No",
         _ => null
    };

    public static string GetCloseButtonText(ContentDialogButtons btns) => btns switch
    {
        ContentDialogButtons.YesNo => "No",
        ContentDialogButtons.YesNoCancel or
        ContentDialogButtons.OKCancel or
        _ => "Cancel"
    };

    public ContentDialogButtons DialogButtons { get; set; }

    public abstract Task<IContentDialogResult> ShowAsync();

    //object? IContentDialogAware.Title { get => title; set => title = value; }
   // object? IContentDialogAware.DialogContent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}

/// <summary>
/// use for stuff like simple message box
/// </summary>
/// <param name="primaryBtnTxt"></param>
/// <param name="closebtnTxt"></param>
/// <param name="content"></param>
/// <param name="secondaryBtnTxt"></param>
/// <param name="title"></param>
public class DefaultContentDialogView(ContentDialogButtons btns,
    object content,
    object? _title = null,
    string? primaryBtnTxt = null,
    string? secondaryBtnTxt = null,
    string? closebtnTxt = null) :
    ContentDialogAwareBase(btns, primaryBtnTxt, secondaryBtnTxt, closebtnTxt),
    IContentDialogAware
{
    public new object? Title => _title ?? ContainerServiceHelper.Current.ContainerProvider?.Resolve<IDefaultContentDialogTitle>();
    public new object? DialogContent => content;

    public override Task<IContentDialogResult> ShowAsync()
    {
        throw new NotImplementedException();
    }
}
