using Chameleon.Interfaces.CookiesExcluded;
using Chameleon.lib.Common.Interfaces.Sys;

namespace Chameleon.Application.Events {
	public class CookiesExcludedDomainEventHandler
        : ICookiesExcludedDomainEventHandler
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ICookiesExcludedDomainService _cookiesExcludedDomainService;

        public CookiesExcludedDomainEventHandler(
            IEventAggregator eventAggregator,
            ICookiesExcludedDomainService cookiesExcludedDomainService
            )
        {
            _eventAggregator = eventAggregator;
            _cookiesExcludedDomainService = cookiesExcludedDomainService;

            _eventAggregator
               .GetEvent<DeleteCookiesExcludedDomainEvent>()
               .Subscribe(args => DeleteCookiesExcludedDomain(args.CookiesExcludedDomain));

            _eventAggregator
               .GetEvent<UpdateCookiesExcludedDomainEvent>()
               .Subscribe(args => UpdateCookiesExcludedDomain(args.CookiesExcludedDomain));

            _eventAggregator
               .GetEvent<CreateCookiesExcludedDomainEvent>()
               .Subscribe(args => CreateCookiesExcludedDomain(args.CookiesExcludedDomain));
        }

        private void CreateCookiesExcludedDomain(ICookiesExcludedDomain cookiesExcludedDomain)
        {
            _cookiesExcludedDomainService.Create(cookiesExcludedDomain);
        }

        private void UpdateCookiesExcludedDomain(ICookiesExcludedDomain cookiesExcludedDomain)
        {
            _cookiesExcludedDomainService.Update(cookiesExcludedDomain);
        }

        private void DeleteCookiesExcludedDomain(ICookiesExcludedDomain cookiesExcludedDomain)
        {
            _cookiesExcludedDomainService.Delete(cookiesExcludedDomain);
        }
    }
}
