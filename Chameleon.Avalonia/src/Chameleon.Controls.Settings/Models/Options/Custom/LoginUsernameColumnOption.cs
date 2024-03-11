using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class LoginUsernameColumnOption : ImportColumnOption
    {
        public LoginUsernameColumnOption()
            : base(ImportColumnOptionType.LoginUserName)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            GetLogin(profile).UserName = input;
        }
    }
}
