namespace Chameleon.App
{
    public class BusinessBaseDto
    {
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string Department { get; set; }
        public string PhoneNumber { get; set; }
        public string WebSite { get; set; }
        public string Notes { get; set; }

        [Identity]
        public int ProfileId { get; set; }
    }
}
