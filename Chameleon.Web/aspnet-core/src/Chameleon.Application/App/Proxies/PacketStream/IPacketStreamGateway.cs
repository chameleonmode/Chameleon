using Abp.Dependency;
using System.Threading.Tasks;

namespace Chameleon.App.PacketStream
{
    public interface IPacketStreamGateway
        : ISingletonDependency
    {
        Task<SubUserBalanceResponseData> ViewSubUserAsync(SubUserNameInputDto input);
        Task<SubUserBalanceResponseData> GetOrCreateSubUserAsync(SubUserNameInputDto input);
        Task<SubUserBalanceResponseData> CreateSubUserAsync(SubUserNameInputDto input);
        Task<SubUserBalanceResponseData> GiveBalanceAsync(UpdateBalanceInputDto input);
        Task<SubUserBalanceResponseData> TakeBalanceAsync(UpdateBalanceInputDto input);
    }
}
