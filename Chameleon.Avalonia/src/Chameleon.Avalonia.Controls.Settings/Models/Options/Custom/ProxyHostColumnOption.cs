using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class ProxyHostColumnOption : ImportColumnOption
    {
        public ProxyHostColumnOption()
            : base(ImportColumnOptionType.ProxyHost)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            profile.Proxy.Host = input;
        }
    }
}
