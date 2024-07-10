using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class MiddleNameColumnOption : ImportColumnOption
    {
        public MiddleNameColumnOption()
            : base(ImportColumnOptionType.MiddleName)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            GetPerson(profile).MiddleName = input;
        }
    }
}
