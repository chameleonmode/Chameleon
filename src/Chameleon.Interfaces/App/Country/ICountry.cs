using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.Country
{
    public interface ICountry 
        : IEntity<int>
    {
        string Name { get; set; }
        string IsoCode2 { get; set; }
        string IsoCode3 { get; set; }
    }
}
