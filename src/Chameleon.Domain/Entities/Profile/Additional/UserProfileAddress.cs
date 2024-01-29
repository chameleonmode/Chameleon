using Chameleon.Interfaces.UserProfiles.Additional;

namespace Chameleon.Domain.Entities
{
    public class UserProfileAddress : IUserProfileAddress
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public int? CountryId { get; set; }
        public string Notes { get; set; }
        public int ProfileId { get; set; }
    }
}
