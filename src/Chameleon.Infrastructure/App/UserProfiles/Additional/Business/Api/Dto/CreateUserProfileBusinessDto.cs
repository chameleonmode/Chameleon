namespace Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional
{
    public class CreateUserProfileBusinessDto
    {
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string Department { get; set; }
        public string PhoneNumber { get; set; }
        public string WebSite { get; set; }
        public string Notes { get; set; }
        public int ProfileId { get; set; }
    }
}
