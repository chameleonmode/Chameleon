using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Chameleon.Common.Helpers;

public static class ConfigHelper
{
    private static string? _lastSelectedBrowser;
    public static string? LastSelectedBrowser
    {
        get => _lastSelectedBrowser ??= GetSetting();
        set => SetSetting(ref _lastSelectedBrowser, value);
    }

    private static int? _lastRunScriptId = null;
    public static int LastRunScriptId
    {
        get => _lastRunScriptId ??= int.Parse(GetSetting());
        set => SetSetting(ref _lastRunScriptId, value);
    }


    private static string? _userScriptsDirectory;
    public static string? UserScriptsDirectory
    {
        get => _userScriptsDirectory ??= GetSetting();
        set => SetSetting(ref _userScriptsDirectory, value);
    }

    public static bool SetSetting<T>([NotNullIfNotNull(nameof(newValue))] ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, newValue))
            return false;

        field = newValue;

        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        config.AppSettings.Settings[propertyName].Value = field.ToString();
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("appSettings");

        return true;
    }

    public static string? GetSetting([CallerMemberName] string? propertyName = null)
    {
        return ConfigurationManager.AppSettings[propertyName];
    }
}
