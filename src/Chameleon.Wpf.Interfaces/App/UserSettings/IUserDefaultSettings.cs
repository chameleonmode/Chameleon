using System.Collections.Generic;
using System.Collections.Specialized;

namespace Chameleon.Interfaces.Settings
{
    public interface IUserDefaultSettings
        : IList<IUserDefaultSetting>
        , INotifyCollectionChanged
    {
    }
}
