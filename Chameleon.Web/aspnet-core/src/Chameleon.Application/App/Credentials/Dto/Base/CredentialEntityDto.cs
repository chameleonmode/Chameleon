using Abp.Application.Services.Dto;

namespace Chameleon.App
{
    public class CredentialEntityDto
        : CredentialBaseDto
        , IEntityDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
