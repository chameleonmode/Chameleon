using Chameleon.Avalonia.Controls.UserProfileView.Models.ProxySettings;
using Chameleon.Avalonia.Controls.UserProfileView.Models.WebBrowser;
using Chameleon.Avalonia.Controls.UserProfileView.Models.WordPress;
using Chameleon.Avalonia.Controls.UserProfileView.Models.Youtube;
using Chameleon.CT.Common.Base;

namespace Chameleon.Avalonia.Controls.UserProfileView.Models.Profile;

public class UserProfileBindable
      : ObservableObjectBase
{
    public event EventHandler<bool> ChangedProperty;

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

    private int _id;
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _notes;
    public string Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    private bool _isFavourite;
    public bool IsFavourite
    {
        get => _isFavourite;
        set => SetProperty(ref _isFavourite, value);
    }

    private string _title;
    public new string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private int? _folderId;
    public int? FolderId
    {
        get => _folderId;
        set => SetProperty(ref _folderId, value);
    }
}