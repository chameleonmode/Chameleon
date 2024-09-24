using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Environments
{
    public interface IApplicationConfiguration : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        string ApiBaseUrl { get; }
        string ApiSocialAnimalUrl { get; }
        string ApiSocialAnimalUserId { get; }
        string ApiSocialAnimalAuthKey { get; }
    }

    public interface ISocialAnimalConfiguration : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        string ApiSocialAnimalUrl { get; }
        string ApiSocialAnimalUserId { get; }
        string ApiSocialAnimalAuthKey { get; }
    }

    public interface IUrlConfiguration : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        string WebsiteUrl { get; }
        string SupportUrl { get; }
        string FacebookGroupUrl { get; }
        string PricingUrl { get; }
    }
}
