using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface IWebBrowserInstance 
        : ITransientDependency
        , IDisposable
    {
        IUserProfile UserProfile { get; }
        void SetUserProfile(IUserProfile userProfile);

        void Initialize();

        object GetControl();
        void Load(string url);

        bool IsBrowserInitialize { get; }
        string Search { get; set; }

        ICommand SearchCommand { get; }
        ICommand ReloadCommand { get; }
        ICommand ForwardCommand { get; }
        ICommand BackCommand { get; }

        event EventHandler<string> TitleChanged;
        event EventHandler<string> SearchChanged;
        event EventHandler<string> OpenInNewTab;
        event EventHandler<WebBrowserPaintEventArgs> Paint;
        event EventHandler<RoutedEventArgs> Loaded;
        event EventHandler<EventArgs> LayoutUpdated;
        event EventHandler<WebBrowserFrameLoadEndEventArgs> FrameLoadEnd;
        event EventHandler ControlUpdated;

        void ClearCache(bool reloadPage);
        void ImportCookies(IList<IWebBrowserCookie> cookies);
        IList<IWebBrowserCookie> ExportCookies();
        void DeleteCookies(params string[] domains);

        IReadOnlyList<string> UrlHistory { get; }
    }
}
