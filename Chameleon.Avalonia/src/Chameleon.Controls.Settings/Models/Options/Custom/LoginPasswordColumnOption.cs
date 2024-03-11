using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class LoginPasswordColumnOption : ImportColumnOption
    {
        public LoginPasswordColumnOption()
            : base(ImportColumnOptionType.LoginPassword)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            GetLogin(profile).Password = input;
        }
    }
}
