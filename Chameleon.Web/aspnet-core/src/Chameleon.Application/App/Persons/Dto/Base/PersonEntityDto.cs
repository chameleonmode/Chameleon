using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class PersonEntityDto
        : PersonBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
