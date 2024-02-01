using System;

namespace Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional
{
    public class CreateUserProfilePersonDto
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string JobTitle { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Notes { get; set; }
        public int Gender { get; set; }
        public int ProfileId { get; set; }
    }
}
