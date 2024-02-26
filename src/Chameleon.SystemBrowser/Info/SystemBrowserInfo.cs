namespace Chameleon.SystemBrowser
{
    public class SystemBrowserInfo : ISystemBrowserInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string IconPath { get; set; }
        public string Version { get; set; }

        public override string ToString()
        {
            return Name ?? Path;
        }
    }
}
