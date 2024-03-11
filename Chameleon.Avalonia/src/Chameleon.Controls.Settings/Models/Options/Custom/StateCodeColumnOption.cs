using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class StateCodeColumnOption : ImportColumnOption
    {
        public StateCodeColumnOption()
            : base(ImportColumnOptionType.State)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            GetAddress(profile).State = input;
        }
    }
}
