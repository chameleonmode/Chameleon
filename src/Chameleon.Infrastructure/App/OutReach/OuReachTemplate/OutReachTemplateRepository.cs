using AutoMapper;

using Chameleon.Infrastructure.Repositories;
using Chameleon.Interfaces.Repository;
using Chameleon.Domain.Entities;
using Chameleon.Infrastructure.OutReach.Api.Dto;
using Chameleon.Infrastructure.OutReach.Api;
using Chameleon.Interfaces.OutReach;

namespace Chameleon.Infrastructure.OutReach
{
    public class OutReachTemplateRepository
         : Repository<OutReachTemplate,
            IOutReachTemplate,
            int,
            OutReachTemplateDto,
            CreateOutReachTemplateDto,
            OutReachTemplateDto,
            GetAllRequestDto
            >
        , IOutReachTemplateRepository
    {
        public OutReachTemplateRepository(
           IMapper mapper,
           IOutReachTemplateApi apiClient,
           IEventAggregator eventAggregator
           ) : base(mapper, apiClient, eventAggregator)
        {
        }
    }
}
