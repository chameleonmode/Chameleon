using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class LicenseEntityDto
        : LicenseBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
