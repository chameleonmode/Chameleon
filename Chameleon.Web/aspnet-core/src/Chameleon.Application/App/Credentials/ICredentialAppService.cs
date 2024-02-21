using Abp.Application.Services;

namespace Chameleon.App
{
    public interface ICredentialAppService
        : IAsyncCrudAppService<
            CredentialDto,
            int,
            CredentialGetAllRequestDto,
            CreateCredentialDto,
            UpdateCredentialDto
            >
    {
    }
}
