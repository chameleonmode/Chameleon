using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class FirstNameColumnOption : ImportColumnOption
    {
        public FirstNameColumnOption()
            : base(ImportColumnOptionType.FirstName)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            GetPerson(profile).FirstName = input;
        }
    }
}
