using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.Country.Api.Dto
{
    public class CountryDto 
        : IEntityDto<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IsoCode2 { get; set; }
        public string IsoCode3 { get; set; }
    }
}
