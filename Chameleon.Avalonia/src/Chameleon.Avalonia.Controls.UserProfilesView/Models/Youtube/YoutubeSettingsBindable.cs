using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.YouTube;


namespace Chameleon.Avalonia.Controls.UserProfileView.Models.Youtube;
public class YoutubeSettingsBindable : ObservableObjectBase, IYouTubeSettings
{
    private string _oldClientId;
    private string _oldClientSecret;
    public bool IsChanged
    {
        get
        {
            return !string.Equals(_oldClientId, ClientId) ||
                   !string.Equals(_oldClientSecret, ClientSecret);
        }
        set
        {
            if (!value)
            {
                _oldClientId = ClientId;
                _oldClientSecret = ClientSecret;
            }
        }
    }

    private bool _isVisible;
    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    private string _apiKey;
    public string ApiKey
    {
        get => _apiKey;
        set => SetProperty(ref _apiKey, value);
    }

    private string _clientId;
    public string ClientId
    {
        get => _clientId;
        set
        {
            SetProperty(ref _clientId, value);

            if (string.IsNullOrEmpty(_oldClientId))
            {
                _oldClientId = value;
            }
        }
    }

    private string _clientSecret;
    public string ClientSecret
    {
        get => _clientSecret;
        set
        {
            SetProperty(ref _clientSecret, value);

            if (string.IsNullOrEmpty(_oldClientSecret))
            {
                _oldClientSecret = value;
            }
        }
    }
}