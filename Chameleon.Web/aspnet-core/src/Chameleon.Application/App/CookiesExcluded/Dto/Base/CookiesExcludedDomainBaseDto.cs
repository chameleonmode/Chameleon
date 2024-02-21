namespace Chameleon.App
{
    public class CookiesExcludedDomainBaseDto
    {
        public string Domain { get; set; }
        [Identity]
        public int ProfileId { get; set; }
    }
}
