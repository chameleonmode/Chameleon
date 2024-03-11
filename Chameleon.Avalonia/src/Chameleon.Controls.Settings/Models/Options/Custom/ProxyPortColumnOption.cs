using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class ProxyPortColumnOption : ImportColumnOption
    {
        public ProxyPortColumnOption()
            : base(ImportColumnOptionType.ProxyPort)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            // TODO: what should be on error parsing (invlid input - not number)?
            int.TryParse(input, out var port);
            profile.Proxy.Port = port;
        }
    }
}
