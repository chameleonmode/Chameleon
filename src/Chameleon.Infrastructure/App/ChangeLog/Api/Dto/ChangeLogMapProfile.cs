using AutoMapper;
using Chameleon.Interfaces.App.ChangeLog;

namespace Chameleon.Infrastructure.App.ChangeLog.Api.Dto
{
    public class ChangeLogMapProfile : Profile
    {
        public ChangeLogMapProfile()
        {
            var map = CreateMap<ChangeLogDto, IChangeLog>();

            map.ReverseMap();
        }
    }
}
