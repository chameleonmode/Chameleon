namespace Chameleon.App
{
    public class CredentialBaseDto
    {
        public string Title { get; set; }
        public string WebSite { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }

        [Identity]
        public int ProfileId { get; set; }
    }
}
