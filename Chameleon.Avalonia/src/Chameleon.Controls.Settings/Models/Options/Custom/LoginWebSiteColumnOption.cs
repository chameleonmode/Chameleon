using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class LoginWebSiteColumnOption : ImportColumnOption
    {
        public LoginWebSiteColumnOption()
            : base(ImportColumnOptionType.LoginWebSite)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            GetLogin(profile).WebSite = input;
        }
    }
}
