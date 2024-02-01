using Chameleon.Interfaces.Environments;

namespace Chameleon.Infrastructure.Environments
{
    public class ApplicationConfiguration
        : IApplicationConfiguration
    {
        private readonly IApplicationConfigurationManager _configurationManager;

        public ApplicationConfiguration(
            IApplicationConfigurationManager configurationManager
            )
        {
            _configurationManager = configurationManager;
        }

        public string ApiBaseUrl => _configurationManager.Get("apiBaseUrl");

        public string ApiSocialAnimalUrl => _configurationManager.Get("apiSocialAnimalUrl");

        public string ApiSocialAnimalUserId => _configurationManager.Get("apiSocialAnimalUserId");

        public string ApiSocialAnimalAuthKey => _configurationManager.Get("apiSocialAnimalAuthKey");
    }

    public class SocialAnimalConfiguration
        : ISocialAnimalConfiguration
    {
        private readonly IApplicationConfigurationManager _configurationManager;
        public SocialAnimalConfiguration(
            IApplicationConfigurationManager configurationManager
            )
        {
            _configurationManager = configurationManager;
        }

        public string ApiSocialAnimalUrl => _configurationManager.Get("apiSocialAnimalUrl");

        public string ApiSocialAnimalUserId => _configurationManager.Get("apiSocialAnimalUserId");

        public string ApiSocialAnimalAuthKey => _configurationManager.Get("apiSocialAnimalAuthKey");
    }

    public class UrlConfiguration
        : IUrlConfiguration
    {
        private readonly IApplicationConfigurationManager _configurationManager;
        public UrlConfiguration(IApplicationConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        public string WebsiteUrl => _configurationManager.Get("websiteUrl");

        public string SupportUrl => _configurationManager.Get("supportUrl");

        public string FacebookGroupUrl => _configurationManager.Get("facebookGroupUrl");

        public string PricingUrl => _configurationManager.Get("pricingUrl");
    }
}
