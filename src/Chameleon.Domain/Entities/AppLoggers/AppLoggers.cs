using Chameleon.Interfaces.ExceptionOptions;

namespace Chameleon.Domain.Entities
{
    public class AppLoggers
        : ObservableEntities<IAppLogger>
        , IAppLoggers
    {
    }
}
