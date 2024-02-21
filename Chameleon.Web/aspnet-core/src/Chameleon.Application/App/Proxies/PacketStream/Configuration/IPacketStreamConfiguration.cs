using Abp.Dependency;

namespace Chameleon.App.PacketStream
{
    public interface IPacketStreamConfiguration 
        : ISingletonDependency
    {
        string ApiHost { get; }
        string ApiAccessToken { get; }
        string ApiEndpoint { get; }
        string TestUserName { get; }
        string TestUserPassword { get; }
        int UserNameMaxLength { get; }
    }
}
