using Chameleon.Interfaces.Entities;
using Prism.Commands;

namespace Chameleon.Interfaces.Settings
{
    public interface IUserDefaultSettingsInfo: IEntity
    {
        string DefaultUrl { get; set; }
    }
}
