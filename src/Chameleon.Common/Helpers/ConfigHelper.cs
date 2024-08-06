using Chameleon.Interfaces.Environments;
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

    public static Task WriteToAppDir(string fname, string content)
    {
        var settingsFilePath = Path.Combine(ContainerServiceHelper.Resolve<IApplicationEnvironment>().ApplicationDataFolderPath, fname);
        return File.WriteAllTextAsync(settingsFilePath, content);
    }

    public static Task<string> ReadFromAppDir(string fname)
    {
        var settingsFilePath = Path.Combine(ContainerServiceHelper.Resolve<IApplicationEnvironment>().ApplicationDataFolderPath, fname);
        if (!File.Exists(settingsFilePath))
            return Task.FromResult<string>(null);

        return File.ReadAllTextAsync(settingsFilePath);
    }
}
