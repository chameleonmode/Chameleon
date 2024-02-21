using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class ProfileEntityDto
        : ProfileBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
