using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Chameleon.App.Entities;
using Chameleon.App.PacketStream;
using Chameleon.Authorization.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Chameleon.App
{
    public class ProxyCreditGateway
        : IProxyCreditGateway
    {
        private readonly UserManager _userManager;
        private readonly IRepository<ProxyCredit> _repository;
        private readonly IPacketStreamGateway _packetStreamGateway;

        public ProxyCreditGateway(
            UserManager userManager,
            IRepository<ProxyCredit> repository,
            IPacketStreamGateway packetStreamGateway
            )
        {
            _userManager = userManager;
            _repository = repository;
            _packetStreamGateway = packetStreamGateway;
            AbpSession = NullAbpSession.Instance;
        }

        public IAbpSession AbpSession { get; set; }

        public async Task<ProxyCredit> GiveBalanceAsync(decimal amount)
        {
            var user = await GetCurrentUserAsync();
            return await GiveBalanceAsync(user, amount);
        }

        public async Task<ProxyCredit> GiveBalanceAsync(User user, decimal amount)
        {
            await EnsureSubAccountCreated(user);

            var proxySubAccount = await _packetStreamGateway.GiveBalanceAsync(new UpdateBalanceInputDto
            {
                UserName = user.UserName,
                BalanceInCents = GetBalanceInCents(amount)
            });

            return await InsertOrUpdateAsync(user, proxySubAccount);
        }

        private async Task EnsureSubAccountCreated(User user)
        {
            var userName = user.UserName;
            await _packetStreamGateway.GetOrCreateSubUserAsync(new SubUserNameInputDto
            {
                UserName = userName
            });
        }

        private async Task<ProxyCredit> InsertOrUpdateAsync(User user, SubUserBalanceResponseData proxySubAccount)
        {
            var proxyCredit = GetOrDefault(user);
            if (proxyCredit != null)
            {
                proxyCredit.ProxyAuthKey = proxySubAccount.ProxyAuthKey;
                await _repository.UpdateAsync(proxyCredit);
                return proxyCredit;
            }

            proxyCredit = new ProxyCredit
            {
                UserId = user.Id,
                ProxyUserName = proxySubAccount.UserName,
                ProxyAuthKey = proxySubAccount.ProxyAuthKey,
                ProviderType = ProxyProviderType.PacketStream
            };

            proxyCredit.Id = await _repository.InsertAndGetIdAsync(proxyCredit);
            return proxyCredit;
        }

        public async Task<decimal> GetBalanceAsync()
        {
            var user = await GetCurrentUserAsync();
            return await GetBalanceAsync(user);
        }

        public async Task<decimal> GetBalanceAsync(User user)
        {
            var proxySubAccount = await _packetStreamGateway.GetOrCreateSubUserAsync(new SubUserNameInputDto
            {
                UserName = user.UserName
            });

            var balance = GetBalanceInUnits(proxySubAccount.Balance);
            return balance;
        }

        public async Task<ProxyCredit> TakeBalanceAsync(decimal amount)
        {
            var currentUser = await GetCurrentUserAsync();
            return await TakeBalanceAsync(currentUser, amount);
        }

        public async Task<ProxyCredit> TakeBalanceAsync(User user, decimal amount)
        {
            await EnsureSubAccountCreated(user);

            var proxySubAccount = await _packetStreamGateway.TakeBalanceAsync(new UpdateBalanceInputDto
            {
                UserName = user.UserName,
                BalanceInCents = GetBalanceInCents(amount)
            });

            return await InsertOrUpdateAsync(user, proxySubAccount);
        }

        private long GetBalanceInCents(decimal amount)
        {
            return (long)(amount * 100);
        }

        private decimal GetBalanceInUnits(decimal amount)
        {
            return Math.Round(amount / 100, 2);
        }

        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var userId = AbpSession.GetUserId().ToString();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }
            return user;
        }

        private ProxyCredit GetOrDefault(User user)
        {
            var proxyCreditsQuery = _repository.GetAll();
            proxyCreditsQuery = proxyCreditsQuery.FilterByUserId(user.Id);
            return proxyCreditsQuery.FirstOrDefault();
        }
    }
}
