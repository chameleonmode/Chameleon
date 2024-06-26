using System.Configuration;

namespace Chameleon.Avalonia.Controls.UserProfilesView.ViewModels;

public class AppSettingsAutomation
{

    public string LastSelectedBrowser
    {
        get => ConfigurationManager.AppSettings["LastSelectedBrowser"];
        set
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["LastSelectedBrowser"].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }

    public int LastRunScriptId
    {
        get => int.Parse(ConfigurationManager.AppSettings["LastRunScriptId"]);
        set
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["LastRunScriptId"].Value = value.ToString();
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
