using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class PhoneNumberColumnOption : ImportColumnOption
    {
        public PhoneNumberColumnOption()
            : base(ImportColumnOptionType.PhoneNumber)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            GetPerson(profile).PhoneNumber = input;
        }
    }
}
