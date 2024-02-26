namespace Chameleon.SystemBrowser
{
    public interface ISystemBrowserInfo
    {
        string Name { get; }
        string Path { get; }
        string IconPath { get;}
        string Version { get; }
    }
}
