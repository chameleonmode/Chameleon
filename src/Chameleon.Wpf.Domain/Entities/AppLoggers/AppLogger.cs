using Chameleon.Interfaces.ExceptionOptions;

namespace Chameleon.Domain.Entities
{
    public class AppLogger : IAppLogger
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public AppLoggerType AppLoggerType { get; set; }
    }
}
