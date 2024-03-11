using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Controls.ImportExport.Models
{
    public class BirthMonthColumnOption : ImportColumnOption
    {
        public BirthMonthColumnOption()
            : base(ImportColumnOptionType.BirthMonth)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            var person = GetPerson(profile);
            var perconBirthDate = person.BirthDate;
            if (!Int32.TryParse(input, out int birthMonth))
            {
                return;
            }

            if(birthMonth < 1)
            {
                return;
            }

            person.BirthDate = new DateTime(perconBirthDate.Year, birthMonth, perconBirthDate.Day);
        }
    }
}
