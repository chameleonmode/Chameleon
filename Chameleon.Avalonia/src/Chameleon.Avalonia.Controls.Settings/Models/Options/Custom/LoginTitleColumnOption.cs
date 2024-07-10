using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class LoginTitleColumnOption : ImportColumnOption
    {
        public LoginTitleColumnOption()
            : base (ImportColumnOptionType.LoginTitle)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            GetLogin(profile).Title = input;
        }
    }
}
