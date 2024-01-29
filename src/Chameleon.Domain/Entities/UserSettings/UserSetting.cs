using Chameleon.Interfaces.Settings;

namespace Chameleon.Domain.Entities
{
    public class UserSetting : IUserSetting
    {
        public int Id { get; set; }
        public string SmsPvaApiKey { get; set; }
    }
}
