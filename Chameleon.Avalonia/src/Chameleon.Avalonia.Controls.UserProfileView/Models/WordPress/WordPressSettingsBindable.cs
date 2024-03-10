using Chameleon.Interfaces.WordPress;
using Prism.Mvvm;

namespace Chameleon.Avalonia.Controls.UserProfileView.Models.WordPress;

public class WordPressSettingsBindable : BindableBase, IWordPressSettings
{
    private string _baseUrl;
    public string BaseUrl
    {
        get => _baseUrl;
        set => SetProperty(ref _baseUrl, value);
    }

    private string _username;
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    private string _password;
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }
}
