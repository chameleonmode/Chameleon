using Chameleon.Interfaces.Settings;

namespace Chameleon.Domain.Entities
{
    public class UserDefaultSetting : IUserDefaultSetting
    {
        public int Id { get; set; }
        public string DefaultUrl { get; set; }
    }
}
