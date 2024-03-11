using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class ProxyPasswordColumnOption : ImportColumnOption
    {
        public ProxyPasswordColumnOption()
            : base(ImportColumnOptionType.ProxyPassword)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            profile.Proxy.Password = input;
        }
    }
}
