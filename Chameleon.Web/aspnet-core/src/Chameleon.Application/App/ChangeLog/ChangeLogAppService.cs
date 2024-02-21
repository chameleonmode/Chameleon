using System.IO;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public class ChangeLogAppService
        : ChameleonAppServiceBase
        , IChangeLogAppService
    {
        public ChangeLogAppService()
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        public Task<object> GetAll()
        {
            //TODO: Move to appsettings.json
            // or change getting logic data from file directly without API
            // or make full CRUD with db storing
            string path = @"c:\Chameleon\ChangeLog.txt";

            if (!File.Exists(path))
            {
                return Task.FromResult((object)string.Empty);
            }

            string readText = File.ReadAllText(path);

            object result = new { Id = 0, Value = readText };

            return Task.FromResult(result);
        }
    }
}
