using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class LoginNotesColumnOption : ImportColumnOption
    {
        public LoginNotesColumnOption()
            : base(ImportColumnOptionType.LoginNotes)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            GetLogin(profile).Notes = input;
        }
    }
}
