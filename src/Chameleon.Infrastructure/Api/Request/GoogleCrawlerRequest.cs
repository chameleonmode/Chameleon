using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.UserProfiles;
using System;
using System.Net;

namespace Chameleon.Infrastructure.Api
{
    public class GoogleCrawlerRequest : ApiGetRequest
    {
        private new readonly IApplicationConfiguration _configuration;
        private readonly IUserProfile _userProfile;

        public GoogleCrawlerRequest(
            IAuthSession session
            , IApplicationConfiguration configuration
            , IUserProfile userProfile
            ) : base(session, configuration)
        {
            _configuration = configuration;
            _userProfile = userProfile;
        }

        protected override string GetUrl(string url)
        {
            return url;
        }

        protected override void SetAuthHeader() { }

        protected override void InitializeRequest(HttpWebRequest request) 
        {
            if(_userProfile == null)
            {
                return;
            }

            var userProxy = _userProfile.Proxy;
            if (!string.IsNullOrEmpty(userProxy.Host)
                && userProxy.HasUserName
                && !string.IsNullOrWhiteSpace(userProxy.Password))
            {
                Request.Proxy = new WebProxy(userProxy.HostForRequest, userProxy.Port);
                Request.Proxy.Credentials = new NetworkCredential(userProxy.UserName, userProxy.Password);
            }
        }

        public override TResult GetResult<TResult>()
        {
            var body = GetResponseBody();
            var googleCrawlerResponse = new GoogleCrawlerResponse();
            googleCrawlerResponse.Response = body;

            try
            {
                return (TResult)Convert.ChangeType(googleCrawlerResponse, typeof(TResult));
            }
            catch(Exception e)
            {
                return default(TResult);
            }
        }
    }
}
