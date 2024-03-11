using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class CityColumnOption : ImportColumnOption
    {
        public CityColumnOption()
            : base(ImportColumnOptionType.City)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            GetAddress(profile).City = input;
        }
    }
}
