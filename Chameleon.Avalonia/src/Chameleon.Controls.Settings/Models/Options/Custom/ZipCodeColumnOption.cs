using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class ZipCodeColumnOption : ImportColumnOption
    {
        public ZipCodeColumnOption()
            : base(ImportColumnOptionType.Zip)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            GetAddress(profile).Zip = input;
        }
    }
}
