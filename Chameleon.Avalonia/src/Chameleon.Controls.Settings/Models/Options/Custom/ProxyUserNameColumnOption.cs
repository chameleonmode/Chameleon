using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class ProxyUserNameColumnOption : ImportColumnOption
    {
        public ProxyUserNameColumnOption()
            : base(ImportColumnOptionType.ProxyUserName)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            profile.Proxy.UserName = input;
        }
    }
}
