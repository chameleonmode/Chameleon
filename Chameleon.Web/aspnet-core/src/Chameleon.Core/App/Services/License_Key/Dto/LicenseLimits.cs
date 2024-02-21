namespace Chameleon.App.Services.License_Key.Dto
{
    public class LicenseLimits
    {
        public bool HasOutreach { get; }
        public bool HasYouTube { get; }
        public bool HasWordPress { get; }
        public int MaxProfilesCount { get; }
        public ContentDiscoveryLimits ContentDiscoveryLimits { get; } = new ContentDiscoveryLimits(true, true, true, true, int.MaxValue);
        public int MaxAssistantsCount { get; }

        public LicenseLimits(LicenseType licenseType)
        {
            switch (licenseType)
            {
                case LicenseType.EXPLORER:
                    {
                        MaxProfilesCount = 50;
                        MaxAssistantsCount = 5;
                        break;
                    }
                case LicenseType.CHAMPION:
                    {
                        MaxProfilesCount = 150;
                        HasOutreach = true;
                        MaxAssistantsCount = 15;
                        break;
                    }
                case LicenseType.SUPERHERO:
                    {
                        MaxProfilesCount = 350;
                        HasOutreach = true;
                        HasYouTube = true;
                        HasWordPress = true;
                        MaxAssistantsCount = 25;
                        break;
                    }
                case LicenseType.ENTERPRISE:
                    {
                        MaxProfilesCount = int.MaxValue;
                        HasOutreach = true;
                        HasYouTube = true;
                        HasWordPress = true;
                        MaxAssistantsCount = int.MaxValue;
                        break;
                    }
                case LicenseType.HATCHLING:
                    {
                        MaxProfilesCount = 2;
                        HasOutreach = true;
                        HasYouTube = true;
                        HasWordPress = true;
                        ContentDiscoveryLimits.HasProspector = true;
                        ContentDiscoveryLimits.HasProspectorContent = false;
                        ContentDiscoveryLimits.HasSocials = true;
                        ContentDiscoveryLimits.HasSocialsContent = false;
                        ContentDiscoveryLimits.MaxRssCount = 3;
                        MaxAssistantsCount = 1;
                        break;
                    }
            }
        }
    }
}
