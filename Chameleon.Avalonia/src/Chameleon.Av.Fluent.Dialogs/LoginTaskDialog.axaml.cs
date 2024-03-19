using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Chameleon.Av.Fluent.Common.Services;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Dialogs.ViewModels;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Xml.Linq;

namespace Chameleon.Av.Fluent.Dialogs;

public partial class LoginTaskDialog : UserControl, ILoginTaskDialog
{
    bool _loaded = false;
    TaskDialog _apiInActionTD;
    //LoginTaskDialogContent _content;

    public LoginTaskDialog()
    {
        InitializeComponent();
        _apiInActionTD = this.FindControl<TaskDialog>(Chameleon.Common.Regions.DialogNames.LoginDialog) ?? new TaskDialog();
       // _content = new LoginTaskDialogContent();

        DataContext = ContainerServiceHelper.Resolve<IAuthTaskDialogViewModel>();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
    }

    // public Control _xamlOwner;
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (!Design.IsDesignMode)
        {
            _loaded = true;
           // Show();
        }
        //  td.XamlRoot = this as Visual;

        //var result = await td.ShowAsync();// ApplicationHelper.GetMainWindow());
    }

    private async void Show()
    {
        var td = new TaskDialog
        {
            Title = _apiInActionTD.Title,
            Header = _apiInActionTD.Header,
            SubHeader = _apiInActionTD.SubHeader,
            Content =null,// _content, //_apiInActionTD.Content,
            IconSource = _apiInActionTD.IconSource,
            ShowProgressBar = _apiInActionTD.ShowProgressBar,
            FooterVisibility = _apiInActionTD.FooterVisibility,
            IsFooterExpanded = _apiInActionTD.IsFooterExpanded,
            Footer = _apiInActionTD.Content,
            DataContext = _apiInActionTD.DataContext,
        };

        if (_apiInActionTD.ShowProgressBar)
        {
            //td.SetProgressBarState(50, _progressFlags);
        }

        //td.Commands.Add(_apiInActionTD.Commands[0]);

        for (int i = 0; i < _apiInActionTD.Buttons.Count; i++)
        {
            td.Buttons.Add(_apiInActionTD.Buttons[i]);
        }

        td.XamlRoot =this;
        var result = await td.ShowAsync(false);
    }

    //private void InitializeComponent()
    //{
    //    AvaloniaXamlLoader.Load(this);
    //}

    public T? FindTControl<T>(string name) where T : class
    {
        var nameScope = this.FindNameScope();
        var res = nameScope?.Find<T>(name);

        if (res is TaskDialog td)
            td.XamlRoot = VisualRoot as Visual;

        return res;
        //return this.FindControl<TaskDialog>(name) as T;
    }

    public async Task<object?> ShowTDialogAsync(string name)
    {
        var children = ApplicationHelper.GetMainWindow()?.GetVisualChildren();
        Visual? root = (ApplicationHelper.GetMainWindow().Content as Visual) ?? ApplicationHelper.GetToplevetVisual();// GetWindowRoot(ApplicationHelper.GetMainWindow()); //children?.LastOrDefault()?.GetVisualChildren().LastOrDefault()?.GetVisualChildren().LastOrDefault() ?? ApplicationHelper.GetToplevetVisual();

        //while (!_loaded)
        //    await Task.Delay(500);

        ////DialogManager.GetVisualForContext(ApplicationHelper.GetMainWindow());
        //var td = this.FindControl<TaskDialog>(name);
        ////(((this.Content as Border).Child as Border).Child as TaskDialog).XamlRoot;
        //if (td is not null)
        //{             
        //    //td.XamlRoot ??= ApplicationHelper.GetToplevetVisual();

        //    return await td.ShowAsync();
        //}
        //return null;

        var td = new TaskDialog
        {
            Title = _apiInActionTD.Title,
            Header = _apiInActionTD.Header,
            SubHeader = _apiInActionTD.SubHeader,
            Content = null, //_apiInActionTD.Content,
            IconSource = _apiInActionTD.IconSource,
            ShowProgressBar = _apiInActionTD.ShowProgressBar,
            FooterVisibility = _apiInActionTD.FooterVisibility,
            IsFooterExpanded = _apiInActionTD.IsFooterExpanded,
            Footer = _apiInActionTD.Content,
            DataContext = _apiInActionTD.DataContext,
        };

        if (_apiInActionTD.ShowProgressBar)
        {
            //td.SetProgressBarState(50, _progressFlags);
        }

        //td.Commands.Add(_apiInActionTD.Commands[0]);

        for (int i = 0; i < _apiInActionTD.Buttons.Count; i++)
        {
            td.Buttons.Add(_apiInActionTD.Buttons[i]);
        }

        td.XamlRoot = root;
        var result = await td.ShowAsync(false);
        return result;
    }

    //private Visual? GetWindowRoot(Visual? visual)
    //{
    //    if (visual?.GetVisualRoot() == ApplicationHelper.GetToplevetVisual())
    //        return GetWindowRoot(visual?.GetVisualChildren().LastOrDefault());

    //    return visual?.GetVisualRoot() as Visual;
    //}
}