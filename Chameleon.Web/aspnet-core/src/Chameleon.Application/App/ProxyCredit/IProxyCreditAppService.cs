using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Chameleon.App.Shared.Proxies;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public interface IProxyCreditAppService
        : IAsyncCrudAppService<
            ProxyCreditDto,
            int,
            PagedAndSortedResultRequestDto,
            CreateProxyCreditDto,
            UpdateProxyCreditDto
            >
    {
        Task<ProxyCreditDto> GetCredits();
        Task<ProxyCreditDto> BuyCredits(BuyCreditsDto input);
        Task<ProxyCreditDto> ReduceCredits(ReduceProxyCreditDto input);
        Task<ProxyCreditDto> GiveCredits(GiveProxyCreditDto input);
        Task<BuyCreditsOrderDto> CreateOrder(CreateBuyCreditsOrderDto input);
        Task<ProxyCreditDto> AddCredits(AddProxyCreditDto input);
    }
}
