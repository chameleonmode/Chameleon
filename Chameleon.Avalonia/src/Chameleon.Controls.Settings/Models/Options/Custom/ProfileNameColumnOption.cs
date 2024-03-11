using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class ProfileNameColumnOption : ImportColumnOption
    {
        public ProfileNameColumnOption()
            : base(ImportColumnOptionType.ProfileName)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            profile.Title = input;
        }
    }
}
