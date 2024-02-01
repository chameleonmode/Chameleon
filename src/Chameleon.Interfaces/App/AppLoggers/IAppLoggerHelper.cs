using Chameleon.Interfaces.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Interfaces.ExceptionOptions
{
    public interface IAppLoggerHelper
        : ISingletonDependency
    {
        void LogWarning(string message);
        void LogError(string message);
    }
}
