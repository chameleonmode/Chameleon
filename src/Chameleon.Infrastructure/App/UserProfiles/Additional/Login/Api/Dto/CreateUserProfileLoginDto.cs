namespace Chameleon.Infrastructure.UserProfiles.Api.Dto.Additional
{
    public class CreateUserProfileLoginDto
    {
        public string Title { get; set; }
        public string WebSite { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }
        public int ProfileId { get; set; }
    }
}
