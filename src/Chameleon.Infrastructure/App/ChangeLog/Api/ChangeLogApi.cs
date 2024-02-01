using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.App.ChangeLog.Api.Dto;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.App.ChangeLog.Api
{
    public class ChangeLogApi
         : ApiLayer<ChangeLogDto>,
          IChangeLogApi
    {
        public ChangeLogApi(IApiClient apiClient)
            : base(apiClient, "changelog")
        {}
    }
}
