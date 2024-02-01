using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Environments
{
    public interface IApplicationConfiguration : ISingletonDependency
    {
        string ApiBaseUrl { get; }
        string ApiSocialAnimalUrl { get; }
        string ApiSocialAnimalUserId { get; }
        string ApiSocialAnimalAuthKey { get; }
    }

    public interface ISocialAnimalConfiguration : ISingletonDependency
    {
        string ApiSocialAnimalUrl { get; }
        string ApiSocialAnimalUserId { get; }
        string ApiSocialAnimalAuthKey { get; }
    }

    public interface IUrlConfiguration : ISingletonDependency
    {
        string WebsiteUrl { get; }
        string SupportUrl { get; }
        string FacebookGroupUrl { get; }
        string PricingUrl { get; }
    }
}
