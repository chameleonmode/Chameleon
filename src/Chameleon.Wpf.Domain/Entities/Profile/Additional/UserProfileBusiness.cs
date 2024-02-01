using Chameleon.Interfaces.UserProfiles.Additional;

namespace Chameleon.Domain.Entities
{
    public class UserProfileBusiness 
        : IUserProfileBusiness
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string Department { get; set; }
        public string PhoneNumber { get; set; }
        public string WebSite { get; set; }
        public string Notes { get; set; }
        public int ProfileId { get; set; }
    }
}
