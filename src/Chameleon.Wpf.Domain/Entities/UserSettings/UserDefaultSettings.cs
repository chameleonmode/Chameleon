using Chameleon.Interfaces.Settings;

namespace Chameleon.Domain.Entities
{
    public class UserDefaultSettings
        : ObservableEntities<IUserDefaultSetting>
        , IUserDefaultSettings
    {
    }
}
