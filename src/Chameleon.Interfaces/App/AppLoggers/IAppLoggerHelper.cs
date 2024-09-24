using Chameleon.Interfaces.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Interfaces.ExceptionOptions
{
    public interface IAppLoggerHelper
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        void LogWarning(string message);
        void LogError(string message);
    }
}
