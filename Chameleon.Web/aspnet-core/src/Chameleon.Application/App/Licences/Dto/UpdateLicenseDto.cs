using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class UpdateLicenseDto
        : IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
