using Chameleon.Common.Extensions;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.UserProfiles.Additional;
using System.Linq;

namespace Chameleon.Controls.ImportExport.Models
{
    public abstract class ImportColumnOption : IImportColumnOption
    {
        public ImportColumnOption(ImportColumnOptionType optionType)
        {
            Type = optionType;
            Title = optionType.GetDescription();
        }

        public ImportColumnOptionType Type { get; }

        private bool _isUsed;
        public bool IsUsed 
        { 
            get => _isUsed;
            set => _isUsed = value;
        }

        private string _title;
        public string Title 
        { 
            get => _title;
            set => _title = value;
        }

        public abstract void Map(IUserProfile profile, string input);

        protected IUserProfilePerson GetPerson(IUserProfile profile)
        {
            var person = profile.Persons.FirstOrDefault();
            if (person == null)
            {
                person = new UserProfilePerson
                {
                    ProfileId = profile.Id
                };
                profile.Persons.Add(person);
            }
            return person;
        }

        protected IUserProfileAddress GetAddress(IUserProfile profile)
        {
            var address = profile.Addresses.FirstOrDefault();
            if(address == null)
            {
                address = new UserProfileAddress
                {
                    ProfileId = profile.Id
                };
                profile.Addresses.Add(address);
            }
            return address;
        }

        protected IUserProfileLogin GetLogin(IUserProfile profile)
        {
            var login = profile.Logins.FirstOrDefault();
            if (login == null)
            {
                login = new UserProfileLogin
                {
                    ProfileId = profile.Id
                };
                profile.Logins.Add(login);
            }
            return login;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
