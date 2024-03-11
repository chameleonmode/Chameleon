using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Controls.ImportExport.Models
{
    public class BirthYearColumnOption : ImportColumnOption
    {
        public BirthYearColumnOption()
            : base(ImportColumnOptionType.BirthYear)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            var person = GetPerson(profile);
            var perconBirthDate = person.BirthDate;
            if (!Int32.TryParse(input, out int birthYear))
            {
                return;
            }

            if(birthYear < 1)
            {
                return;
            }

            person.BirthDate = new DateTime(birthYear, perconBirthDate.Month, perconBirthDate.Day);
        }
    }
}
