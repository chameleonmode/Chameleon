namespace Chameleon.App
{
    public class AddressBaseDto
    {
        public string Title { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Notes { get; set; }
        public int? CountryId { get; set; }

        [Identity]
        public int ProfileId { get; set; }
    }
}
