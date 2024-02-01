using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.App.ChangeLog.Api.Dto
{
    public class ChangeLogDto
        : IEntityDto<int>
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
