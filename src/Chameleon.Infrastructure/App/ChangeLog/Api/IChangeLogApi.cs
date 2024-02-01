using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.App.ChangeLog.Api.Dto;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.App.ChangeLog.Api
{
    public interface IChangeLogApi
       : IApiLayer<ChangeLogDto>,
        ISingletonDependency
    {
    }
}
