using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading;
using Abp.UI;
using Chameleon.App.Entities;
using Chameleon.App.Shared.Proxies;
using Chameleon.Authorization;
using Chameleon.Authorization.Roles;
using Chameleon.Authorization.Users;
using Chameleon.MultiTenancy.Payments;
using Chameleon.Payments;
using Chameleon.Payments.Stripe;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Chameleon.App
{
    [AbpAuthorize, AbpAuthorize(PermissionNames.Pages_ProxyCredit)]
    public partial class ProxyCreditAppService
        : AsyncCrudAppService<
            ProxyCredit,
            ProxyCreditDto,
            int,
            PagedAndSortedResultRequestDto,
            CreateProxyCreditDto,
            UpdateProxyCreditDto
            >
        , IProxyCreditAppService
    {
        private readonly IProxyCreditGateway _proxyCreditGateway;
        private readonly IRepository<ProxyCreditOrder, Guid> _orderRepository;
        private readonly IRepository<Payment, long> _paymentRepository;
        private readonly IPaymentGatewayManager _paymentGatewayManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager _userManager;

        public ProxyCreditAppService(
            IRepository<ProxyCredit> repository,
            IProxyCreditGateway proxyCreditGateway,
            IRepository<ProxyCreditOrder, Guid> orderRepository,
            IRepository<Payment, long> paymentRepository,
            IStripeGatewayManager paymentGatewayManager,
            IHttpContextAccessor httpContextAccessor,
            UserManager userManager
            ) : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
            CreatePermissionName = PermissionNames.Pages_ProxyCredits_Create;
            UpdatePermissionName = PermissionNames.Pages_ProxyCredits_Update;

            _proxyCreditGateway = proxyCreditGateway;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _paymentGatewayManager = paymentGatewayManager;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public override Task<ProxyCreditDto> CreateAsync(CreateProxyCreditDto input)
        {
            throw new NotImplementedException();
        }

        public async Task<ProxyCreditDto> ReduceCredits(ReduceProxyCreditDto input)
        {
            try
            {
                await VerifyCurrentUserIsAdmin();
                var user = await _userManager.GetUserByIdAsync(input.UserId);
                var entity = await _proxyCreditGateway.TakeBalanceAsync(user, input.Amount);
                var entityDto = await MapToEntityDtoAsync(user, entity);
                return entityDto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<ProxyCreditDto> GiveCredits(GiveProxyCreditDto input)
        {
            try
            {
                await VerifyCurrentUserIsAdmin();
                var user = await _userManager.GetUserByIdAsync(input.UserId);
                var entity = await _proxyCreditGateway.GiveBalanceAsync(user, input.Amount);
                var entityDto = await MapToEntityDtoAsync(user, entity);
                return entityDto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private async Task VerifyCurrentUserIsAdmin()
        {
            var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
            if (!await _userManager.IsInRoleAsync(user, StaticRoleNames.Tenants.Admin))
            {
                throw new AbpAuthorizationException();
            }
        }

        public async Task<BuyCreditsOrderDto> CreateOrder(CreateBuyCreditsOrderDto input)
        {
            var entity = new ProxyCreditOrder
            {
                Id = Guid.NewGuid(),
                UserId = AbpSession.UserId.Value,
                Amount = input.Amount
            };

            await _orderRepository.InsertAsync(entity);

            return new BuyCreditsOrderDto
            {
                Id = entity.Id,
                Amount = input.Amount,
                Url = GetBuyCreditUrl(entity.Id)
            };
        }

        private string GetBuyCreditUrl(Guid id)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var buyUrl = $"{request.Scheme}{Uri.SchemeDelimiter}{request.Host}/buyCredits?id={id}";
            return buyUrl;
        }

        public override Task<ProxyCreditDto> UpdateAsync(UpdateProxyCreditDto input)
        {
            throw new NotImplementedException();
        }

        protected override IQueryable<ProxyCredit> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            return CreateQuery();
        }

        protected override Task<ProxyCredit> GetEntityByIdAsync(int id)
        {
            return Task.Run(() => CreateQuery()
                .FirstOrDefault(entity => entity.Id == id)
                );
        }

        private IQueryable<ProxyCredit> CreateQuery()
        {
            var query = Repository.GetAll();
            query = query.FilterByUserId(AbpSession);
            return query;
        }

        public async Task<ProxyCreditDto> GetCredits()
        {
            var entity = GetOrDefault();
            if (entity == null)
            {
                return new ProxyCreditDto();
            }

            var entityDto = await MapToEntityDtoAsync(entity);
            return entityDto;
        }

        private async Task<ProxyCreditDto> MapToEntityDtoAsync(ProxyCredit entity)
        {
            var entityDto = base.MapToEntityDto(entity);
            entityDto.Amount = await _proxyCreditGateway.GetBalanceAsync();
            return entityDto;
        }

        private async Task<ProxyCreditDto> MapToEntityDtoAsync(User user, ProxyCredit entity)
        {
            var entityDto = base.MapToEntityDto(entity);
            entityDto.Amount = await _proxyCreditGateway.GetBalanceAsync(user);
            return entityDto;
        }

        protected override ProxyCreditDto MapToEntityDto(ProxyCredit entity)
        {
            return AsyncHelper.RunSync(() => MapToEntityDtoAsync(entity));
        }

        public async Task<ProxyCreditDto> BuyCredits(BuyCreditsDto input)
        {
            var proxyCredit = await _proxyCreditGateway.GiveBalanceAsync(input.Amount);
            try
            {
                await ChargeAsync(input);
                return MapToEntityDto(proxyCredit);
            }
            catch (Exception ex)
            {
                await _proxyCreditGateway.TakeBalanceAsync(input.Amount);
                throw new UserFriendlyException(ex.Message);
            }
        }

        private async Task<Payment> ChargeAsync(BuyCreditsDto input)
        {
            var creditCard = new PaymentCardOption
            {
                Number = input.Number,
                ExpYear = input.ExpYear,
                ExpMonth = input.ExpMonth,
                Cvc = input.Cvc
            };

            var result = await _paymentGatewayManager.CreateCharge(
                creditCard, input.Amount, "Buy Proxy Credits"
                );

            var payment = new Payment
            {
                Gateway = PaymentGatewayType.Stripe,
                ExternalPaymentId = result.Id
            };
            payment.SetAsPaid();

            await _paymentRepository.InsertAsync(payment);
            return payment;
        }

        private ProxyCredit GetOrDefault()
        {
            var proxyCreditsQuery = Repository.GetAll();
            proxyCreditsQuery = proxyCreditsQuery.FilterByUserId(AbpSession);
            return proxyCreditsQuery.FirstOrDefault();
        }
        public async Task<ProxyCreditDto> AddCredits(AddProxyCreditDto input)
        {
            try
            {
                await VerifyCurrentUserIsAdmin();

                User user = null;

                using (UnitOfWorkManager.Current.DisableFilter(
                    AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant
                ))
                {
                    user = await _userManager.FindByNameOrEmailAsync(input.Email);
                }

                if (user == null)
                {
                    throw new UserFriendlyException($"Chameleon user with {input.Email} not found.");
                }

                var entity = await _proxyCreditGateway.GiveBalanceAsync(user, input.Amount);
                var entityDto = await MapToEntityDtoAsync(user, entity);

                return entityDto;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}

