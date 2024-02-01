using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.Settings
{
    public interface IUserDefaultSettingsInfo: IEntity
    {
        string DefaultUrl { get; set; }
    }
}
