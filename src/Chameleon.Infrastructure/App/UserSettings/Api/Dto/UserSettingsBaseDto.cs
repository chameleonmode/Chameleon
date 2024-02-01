
namespace Chameleon.Infrastructure.UserSettings
{
    public class UserSettingsBaseDto
    {
        public string SmsPvaApiKey { get; set; }

        public override string ToString()
        {
            return SmsPvaApiKey;
        }
    }
}
