using Abp.Domain.Entities;

namespace Chameleon.App.Entities
{
    public class Country : Entity
    {
        public string Name { get; set; }
        public bool IsMetric { get; set; }
        public string ISOCode2 { get; set; }
        public string ISOCode3 { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
