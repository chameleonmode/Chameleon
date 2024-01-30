using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.Settings
{
    public interface IUserSettingsInfo : IEntity
    {
        string SmsPvaApiKey { get; set; }
    }
}
