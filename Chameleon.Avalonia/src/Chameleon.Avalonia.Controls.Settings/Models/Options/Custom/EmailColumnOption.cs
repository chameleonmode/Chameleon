using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class EmailColumnOption : ImportColumnOption
    {
        public EmailColumnOption()
            : base(ImportColumnOptionType.Email)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            GetPerson(profile).Email = input;
        }
    }
}
