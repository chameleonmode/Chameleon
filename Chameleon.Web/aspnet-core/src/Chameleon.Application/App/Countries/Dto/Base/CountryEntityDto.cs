using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class CountryEntityDto
        : CountryBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
