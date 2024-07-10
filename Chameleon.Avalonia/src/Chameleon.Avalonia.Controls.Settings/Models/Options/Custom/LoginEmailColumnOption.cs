using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class LoginEmailColumnOption : ImportColumnOption
    {
        public LoginEmailColumnOption()
            : base(ImportColumnOptionType.LoginEmail)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            GetLogin(profile).Email = input;
        }
    }
}
