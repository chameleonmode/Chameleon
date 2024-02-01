
namespace Chameleon.Infrastructure.UserSettings
{
    public class UserDefaultSettingsBaseDto
    {
        public string DefaultUrl { get; set; }

        public override string ToString()
        {
            return DefaultUrl;
        }
    }
}
