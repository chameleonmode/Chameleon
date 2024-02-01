using Chameleon.Domain.Entities;
using Chameleon.Interfaces.ExceptionOptions;

namespace Chameleon.Infrastructure.ExceptionOptions
{
    public class AppLoggerHelper : IAppLoggerHelper
    {
        private readonly IAppLoggerService _appLoggerService;

        public AppLoggerHelper(IAppLoggerService appLoggerService)
        {
            _appLoggerService = appLoggerService;
        }

        public void LogError(string message)
        {
            CreateLog(message, AppLoggerType.Error);
        }

        public void LogWarning(string message)
        {
            CreateLog(message, AppLoggerType.Warning);
        }

        private void CreateLog(string message, AppLoggerType type)
        {
            var log = new AppLogger
            {
                Message = message,
                AppLoggerType = type
            };

            _appLoggerService.Create(log);
        }
    }
}
