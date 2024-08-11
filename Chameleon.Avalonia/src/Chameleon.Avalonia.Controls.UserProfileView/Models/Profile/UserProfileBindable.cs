using Chameleon.Avalonia.Controls.UserProfileView.Models.ProxySettings;
using Chameleon.Avalonia.Controls.UserProfileView.Models.WebBrowser;
using Chameleon.Avalonia.Controls.UserProfileView.Models.WordPress;
using Chameleon.Avalonia.Controls.UserProfileView.Models.Youtube;
using Chameleon.CT.Common.Base;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.UserProfiles;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.Avalonia.Controls.UserProfileView.Models.Profile;

//[INotifyPropertyChanged]
public partial class UserProfileBindable : ObservableObject

{
    public event EventHandler<bool> ChangedProperty;

    [ObservableProperty]
    private int _id;
    [ObservableProperty]
    private string _notes;
    [ObservableProperty]
    private bool _isFavourite;
    [ObservableProperty]
    private string _title;
    [ObservableProperty]
    private int? _folderId;

    public UserProfileBindable()
    {
        PropertyChanged += UserProfileBindablePropertyChanged;
    }

    private void UserProfileBindablePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        ChangedProperty?.Invoke(this, true);
    }

    public void SetChangedProperty(bool value)
    {
        ChangedProperty?.Invoke(this, value);
    }

    private ProxySettingsBindable _proxy;
    public ProxySettingsBindable Proxy
    {
        get => _proxy;
        set
        {
            if (SetProperty(ref _proxy, value))
            {
                _proxy.PropertyChanged += UserProfileBindablePropertyChanged;
            }
        }
    }

    private WebBrowserSettingsBindable _webBrowser;
    public WebBrowserSettingsBindable WebBrowser
    {
        get => _webBrowser;
        set
        {
            if (SetProperty(ref _webBrowser, value))
            {
                _webBrowser.PropertyChanged += UserProfileBindablePropertyChanged;
            }
        }
    }

    private YoutubeSettingsBindable _youtubeSettings;
    public YoutubeSettingsBindable YoutubeSettings
    {
        get => _youtubeSettings;
        set
        {
            if (SetProperty(ref _youtubeSettings, value))
            {
                _youtubeSettings.PropertyChanged += UserProfileBindablePropertyChanged;
            }
        }
    }

    private WordPressSettingsBindable _wordPressSettings;
    public WordPressSettingsBindable WordPressSettings
    {
        get => _wordPressSettings;
        set
        {
            if (SetProperty(ref _wordPressSettings, value))
            {
                _wordPressSettings.PropertyChanged += UserProfileBindablePropertyChanged;
            }
        }
    }
}