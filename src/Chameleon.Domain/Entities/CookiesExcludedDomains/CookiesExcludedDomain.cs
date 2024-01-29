using Chameleon.Interfaces.CookiesExcluded;

namespace Chameleon.Domain.Entities
{
    public class CookiesExcludedDomain
        : ICookiesExcludedDomain
    {
        public string Domain { get; set; } = string.Empty;
        public int ProfileId { get; set; }
        public int Id { get; set; }
    }
}
