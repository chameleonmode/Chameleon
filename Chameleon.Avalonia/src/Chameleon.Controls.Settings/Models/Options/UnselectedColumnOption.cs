using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class UnselectedColumnOption : ImportColumnOption
    {
        private static UnselectedColumnOption _instance = new UnselectedColumnOption();
        public static UnselectedColumnOption Instance => _instance;

        private UnselectedColumnOption()
            : base (ImportColumnOptionType.None)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
        }
    }
}
