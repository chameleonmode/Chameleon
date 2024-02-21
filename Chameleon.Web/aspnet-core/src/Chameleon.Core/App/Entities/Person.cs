using Abp.Domain.Entities.Auditing;
using System;

namespace Chameleon.App.Entities
{
    public class Person 
        : FullAuditedEntity
        , IMustHaveProfile
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

        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
