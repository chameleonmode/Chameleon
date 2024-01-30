using Chameleon.Interfaces.Entities;

namespace Chameleon.Interfaces.ExceptionOptions
{
    public interface IAppLoggerInfo : IEntity
    {
        string Message { get; set; }
        AppLoggerType AppLoggerType { get; set; }
    }
}
