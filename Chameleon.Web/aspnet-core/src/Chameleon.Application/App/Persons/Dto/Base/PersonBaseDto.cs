using Chameleon.App.Entities;
using System;

namespace Chameleon.App
{
    public class PersonBaseDto
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
        public GenderType Gender { get; set; }

        [Identity]
        public int ProfileId { get; set; }
    }
}
