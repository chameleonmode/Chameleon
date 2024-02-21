using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class ProfileInfoDto
        : IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public long? CreatorUserId { get; set; }
    }
}
