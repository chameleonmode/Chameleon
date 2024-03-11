using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class StreetColumnOption : ImportColumnOption
    {
        public StreetColumnOption()
            : base(ImportColumnOptionType.Street)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            GetAddress(profile).AddressLine1 = input;
        }
    }
}
