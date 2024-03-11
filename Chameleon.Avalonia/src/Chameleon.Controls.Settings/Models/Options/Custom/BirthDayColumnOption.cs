using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Controls.ImportExport.Models
{
    public class BirthDayColumnOption : ImportColumnOption
    {
        public BirthDayColumnOption()
            : base(ImportColumnOptionType.BirthDay)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            var person = GetPerson(profile);
            var perconBirthDate = person.BirthDate;
            if (!Int32.TryParse(input, out int birthDay))
            {
                return;
            }

            if(birthDay < 1)
            {
                return;
            }

            person.BirthDate = new DateTime(perconBirthDate.Year, perconBirthDate.Month, birthDay);
        }
    }
}
