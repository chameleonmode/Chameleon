using Chameleon.Interfaces.Ioc;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Interfaces.Logger
{
    public interface ILogger : ISingletonDependency
    {
        ILog Log { get; }
    }
}
