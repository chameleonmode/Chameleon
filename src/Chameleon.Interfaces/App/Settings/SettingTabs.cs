namespace Chameleon.Interfaces.Settings
{
    public enum SettingTabs
    {
        None,
        DefaultSettings,
        ProxySettings,
        ProxyCredit,
        PhoneVerification,
        Users,
        ImportExport
    }

    public static class SettingTabsExtensions
    {
        public static string GetString(this SettingTabs me)
        {
            switch (me)
            {
                case SettingTabs.None:
                    return "None";
                case SettingTabs.DefaultSettings:
                    return "DEFAULTS";
                case SettingTabs.ProxySettings:
                    return "PROXY";
                case SettingTabs.ProxyCredit:
                    return "PROXY CREDIT";
                case SettingTabs.PhoneVerification:
                    return "PHONE VERIFICATION";
                case SettingTabs.Users:
                    return "USERS";
                case SettingTabs.ImportExport:
                    return "IMPORT PROFILES";
                default:
                    return "NO VALUE GIVEN";
            }
        }
    }
}