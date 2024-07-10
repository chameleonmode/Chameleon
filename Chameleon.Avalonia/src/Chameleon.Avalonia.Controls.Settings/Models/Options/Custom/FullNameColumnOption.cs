using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Controls.ImportExport.Models
{
    public class FullNameColumnOption : ImportColumnOption
    {
        public FullNameColumnOption()
            : base(ImportColumnOptionType.FullName)
        {
        }

        public override void Map(IUserProfile profile, string input)
        {
            var person = GetPerson(profile);
            var fullNameParts = input.Split(' ');

            person.FirstName = fullNameParts[0];
            if (string.IsNullOrEmpty(person.Title))
            {
                person.Title = input;
            }

            if (fullNameParts.Length == 2)
            {
                person.LastName = fullNameParts[1];
                return;
            }

            if (fullNameParts.Length > 2)
            {
                person.LastName = fullNameParts[1];
                person.MiddleName = fullNameParts[2];
            }
        }
    }
}
