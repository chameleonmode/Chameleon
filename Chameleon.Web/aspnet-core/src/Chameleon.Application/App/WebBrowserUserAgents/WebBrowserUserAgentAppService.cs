using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using Chameleon.App.Entities;
using System.Linq;

namespace Chameleon.App
{
    [AbpAuthorize]
    public class WebBrowserUserAgentAppService
        : AsyncCrudAppService<
            WebBrowserUserAgent,
            WebBrowserUserAgentDto,
            int,
            WebBrowserUserAgentGetAllRequestDto,
            CreateWebBrowserUserAgentDto,
            UpdateWebBrowserUserAgentDto
            >
        , IWebBrowserUserAgentAppService
    {
        public WebBrowserUserAgentAppService(
            IRepository<WebBrowserUserAgent> repository
            ) : base(repository)
        {
            LocalizationSourceName = ChameleonConsts.LocalizationSourceName;
        }

        protected override IQueryable<WebBrowserUserAgent> CreateFilteredQuery(WebBrowserUserAgentGetAllRequestDto input)
        {
            var query = base.CreateFilteredQuery(input);
            query = query.Where(entity =>
                entity.CreatorUserId == null ||
                entity.CreatorUserId == AbpSession.UserId
                );
            return query;
        }

        protected override IQueryable<WebBrowserUserAgent> ApplySorting(IQueryable<WebBrowserUserAgent> query, WebBrowserUserAgentGetAllRequestDto input)
        {
            query = base.ApplySorting(query, input);
            if (input.Sorting.IsNullOrEmpty())
            {
                query = query.OrderBy(entity => entity.CreationTime);
            }
            return query;
        }

        public int GetDefaultUserAgentId()
        {
            var defaultUserAgent = Repository.FirstOrDefault(entity => entity.IsDefault);
            if (defaultUserAgent == null)
            {
                defaultUserAgent = Repository
                .GetAll()
                .Where(entity => entity.CreatorUserId == null)
                .OrderBy(entity => entity.CreationTime)
                .FirstOrDefault();
                if (defaultUserAgent == null)
                {
                    return 0;
                }
                defaultUserAgent.IsDefault = true;
            }

            return defaultUserAgent.Id;
        }

        public void SetDefaultUserAgentId(int id)
        {
            
            var userAgents = Repository.GetAll()
                .Where(entity => entity.CreatorUserId == null);

            var userAgentToSet = userAgents.FirstOrDefault(x => x.Id == id);
            if (userAgentToSet == null)
            {
                throw new UserFriendlyException($"WebBrowserUserAgent with id-{id} not global and couldn't be set as default");
            }

            foreach (var userAgent in userAgents)
            {
                if (userAgent.Id == id)
                {
                    userAgent.IsDefault = true;
                    continue;
                }
                userAgent.IsDefault = false;
            }
        }
    }
}
