using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.App.Users.PrimaryUser.Api.Dto;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Infrastructure.App.Users.PrimaryUser.Api
{
    public interface IPrimaryUserApi
        : IApiLayer<PrimaryUserDto>
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        void MarkGuidedTourDone();
    }
}
