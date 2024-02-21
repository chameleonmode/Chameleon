using Chameleon.Interfaces.Services;

namespace Chameleon.Maui.Toolkit.Services;
public class MauiSettingsService  : ISettingsService
{
    #region Setting Constants 
    private const string AccessToken = "access_token";
    private readonly string AccessTokenDefault = string.Empty;
    #endregion

    #region Settings Properties

    public string AuthAccessToken
    {
        get => Preferences.Get(AccessToken, AccessTokenDefault);
        set => Preferences.Set(AccessToken, value);
    }

    #endregion
}
