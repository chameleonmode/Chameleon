using Chameleon.Interfaces.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Interfaces.Logger
{
    public interface ILogger : ISingletonDependency
    {
        //TODO: find replacement for log4net
        //ILog Log { get; }
    }
}
