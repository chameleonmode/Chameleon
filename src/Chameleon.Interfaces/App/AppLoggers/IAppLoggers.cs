using System.Collections.Generic;
using System.Collections.Specialized;

namespace Chameleon.Interfaces.ExceptionOptions
{
    public interface IAppLoggers
        : IList<IAppLogger>
        , INotifyCollectionChanged
    {
    }
}
