namespace Chameleon.Interfaces.Settings
{
    public interface ILoginSettings
    {
        string LoginName { get; }
        string LicenseKey { get; }
        void Set(string loginName, string licenseKey);
    }
}
