using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Controls.ImportExport.Models
{
    public class BirthDateColumnOption : ImportColumnOption
    {
        public BirthDateColumnOption()
            : base(ImportColumnOptionType.BirthDate)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            if (!DateTime.TryParse(input, out DateTime birthDate))
            {
                return;
            }
            GetPerson(profile).BirthDate = birthDate;
        }
    }
}
