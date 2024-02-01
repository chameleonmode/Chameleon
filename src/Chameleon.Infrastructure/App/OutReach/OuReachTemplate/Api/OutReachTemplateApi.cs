using Chameleon.Infrastructure.Api;
using Chameleon.Infrastructure.OutReach.Api.Dto;
using Chameleon.Interfaces.Api;

namespace Chameleon.Infrastructure.OutReach.Api
{
    public class OutReachTemplateApi
        : ApiLayer<
            OutReachTemplateDto
            , int
            , CreateOutReachTemplateDto
            , OutReachTemplateDto
            >
        , IOutReachTemplateApi
    {
        public OutReachTemplateApi(IApiClient apiClient)
            : base(apiClient, "outreachtemplate")
        { }
    }
}
