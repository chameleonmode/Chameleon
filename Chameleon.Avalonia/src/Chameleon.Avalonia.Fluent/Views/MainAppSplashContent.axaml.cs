using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Chameleon.Av.Fluent.Dialogs;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Dialogs;
using FluentAvalonia.UI.Controls;

namespace Chameleon.Av.Fluent.Views;

public partial class MainAppSplashContent : UserControl
{
    public MainAppSplashContent()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        TargetProgressBar.IsIndeterminate = true;
        if (!Design.IsDesignMode)
         {
           // var td = this.FindControl<TaskDialog>(Chameleon.Common.Regions.DialogNames.LoginDialog);
           // td.DataContext = ContainerServiceHelper.Current.ContainerProvider?.Resolve<IAuthTaskDialogViewModel>();
           // td.XamlRoot = VisualRoot as Visual;
           // td.Opening += (o, e) =>
           // {
           // };
           // await td.ShowAsync();

            //var login = ContainerServiceHelper.Current.ContainerProvider?.Resolve<LoginTaskDialog>();
            //login.Name = "LoginTaskDialogHostDialog";
            //this.VisualChildren.Add(login);
            //(this.Content as Grid).Children.Add(login);
            //var ltd = this.FindControl<LoginTaskDialog>("LoginTaskDialogHostDialog");
            //var td = this.FindControl<TaskDialog>(Chameleon.Common.Regions.DialogNames.LoginDialog);
            //td.XamlRoot = VisualRoot as Visual;
            // td.Opening += (o,e) =>
            // { 
            // };
            //login.UpdateLayout();
            //td.UpdateLayout();
            //login.ApplyStyling();
            //td.ApplyStyling();
            // await td.ShowAsync();
            //var login = ContainerServiceHelper.Current.ContainerProvider?.Resolve<LoginTaskDialog>();
            ////login.Content
            //(((login.Content as Border).Child as Border).Child as TaskDialog).XamlRoot = VisualRoot as Visual;
            //this.VisualChildren.Add(login);
            //ContainerServiceHelper.Current.ContainerProvider?
            // .Resolve<IApplicationStartup>()
            // .Run();
            //var login = ContainerServiceHelper.Current.ContainerProvider?.Resolve<LoginTaskDialog>();
            //var _apiInActionTD = (((login.Content as Border).Child as Border).Child as TaskDialog);
            //var td = new TaskDialog
            //{
            //    Title = _apiInActionTD.Title,
            //    Header = _apiInActionTD.Header,
            //    SubHeader = _apiInActionTD.SubHeader,
            //    Content = _apiInActionTD.Content,
            //    IconSource = _apiInActionTD.IconSource,
            //    ShowProgressBar = _apiInActionTD.ShowProgressBar,
            //    FooterVisibility = _apiInActionTD.FooterVisibility,
            //    IsFooterExpanded = _apiInActionTD.IsFooterExpanded,
            //    Footer = new CheckBox { Content = "Never show me this again" }
            //};

            //if (_apiInActionTD.ShowProgressBar)
            //{
            //    td.SetProgressBarState(50, _progressFlags);
            //}

            //td.Commands.Add(_apiInActionTD.Commands[0]);

            //for (int i = 0; i < _apiInActionTD.Buttons.Count; i++)
            //{
            //    td.Buttons.Add(_apiInActionTD.Buttons[i]);
            //}

            //td.XamlRoot = VisualRoot as Visual;
            //var result = await td.ShowAsync(false);
            // (((login.Content as Border).Child as Border).Child as TaskDialog).XamlRoot = VisualRoot as Visual;
            //  await (((login.Content as Border).Child as Border).Child as TaskDialog).ShowAsync();
        }
    }
}