using Chameleon.Interfaces.Country;

namespace Chameleon.Domain.Entities.Country
{
    public class Country
        : ICountry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IsoCode2 { get; set; }
        public string IsoCode3 { get; set; }
    }
}
