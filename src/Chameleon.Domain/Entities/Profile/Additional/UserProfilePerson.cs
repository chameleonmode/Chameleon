using Chameleon.Interfaces.UserProfiles.Additional;
using System;

namespace Chameleon.Domain.Entities
{
    public class UserProfilePerson 
        : IUserProfilePerson
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public string JobTitle { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime BirthDate { get; set; } = DateTimeOffset.Now.AddYears(-20).DateTime;
        public string BirthPlace { get; set; } = "";
        public string Notes { get; set; } = "";
        public GenderType Gender { get; set; } = GenderType.Female;
        public int ProfileId { get; set; }
    }
}
