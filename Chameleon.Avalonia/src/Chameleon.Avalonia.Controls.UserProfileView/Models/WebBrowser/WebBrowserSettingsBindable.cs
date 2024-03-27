using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.WebBrowser;
using CommunityToolkit.Mvvm.ComponentModel;


namespace Chameleon.Avalonia.Controls.UserProfileView.Models.WebBrowser;

public class WebBrowserSettingsBindable : ObservableObject, IWebBrowserSettings
{
    private bool _webRTC;
    public bool WebRTC
    {
        get => _webRTC;
        set => SetProperty(ref _webRTC, value);
    }

    private bool _webGL;
    public bool WebGL
    {
        get => _webGL;
        set => SetProperty(ref _webGL, value);
    }

    private bool _tracking;
    public bool Tracking
    {
        get => _tracking;
        set => SetProperty(ref _tracking, value);
    }

    private bool _flash;
    public bool Flash
    {
        get => _flash;
        set => SetProperty(ref _flash, value);
    }

    private decimal _canvas;
    public decimal Canvas
    {
        get => _canvas;
        set => SetProperty(ref _canvas, value);
    }

    private int? _userAgentId;
    public int? UserAgentId
    {
        get => _userAgentId;
        set => SetProperty(ref _userAgentId, value);
    }

    public IWebBrowserUserAgent UserAgent { get; set; }
}
