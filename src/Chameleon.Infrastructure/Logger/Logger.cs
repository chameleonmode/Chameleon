using Chameleon.Interfaces.Logger;
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(Watch = true, ConfigFile = "log4net.config")]

namespace Chameleon.Infrastructure.Logger
{   
    public class Logger : ILogger
    {
        private ILog _log;
        public ILog Log
        {
            get
            {
                if(_log == null)
                {
                    _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                }
                return _log;
            }
        }

    }
}
