using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Environments;

namespace Chameleon.Infrastructure.Api
{
    public class ApiSocialAnimalPostRequest : ApiPostRequest
    {
        private new readonly IApplicationConfiguration _configuration;
        public ApiSocialAnimalPostRequest(
            IAuthSession session,
            IApplicationConfiguration configuration
            ) : base(session, configuration)
        {
            _configuration = configuration;
        }

        protected override string GetUrl(string url)
        {
            return _configuration.ApiSocialAnimalUrl;
        }

        protected override void SetAuthHeader()
        {
            Request.Headers["X-Auth-Key"] = _configuration.ApiSocialAnimalAuthKey;
            Request.Headers["X-User-Id"] = _configuration.ApiSocialAnimalUserId;
        }
    }
}
