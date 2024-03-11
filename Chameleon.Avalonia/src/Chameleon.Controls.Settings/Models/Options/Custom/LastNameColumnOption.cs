using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class LastNameColumnOption : ImportColumnOption
    {
        public LastNameColumnOption()
            : base(ImportColumnOptionType.LastName)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            GetPerson(profile).LastName = input;
        }
    }
}
