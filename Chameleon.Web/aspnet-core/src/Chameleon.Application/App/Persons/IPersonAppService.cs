using Abp.Application.Services;

namespace Chameleon.App
{
    public interface IPersonAppService
        : IAsyncCrudAppService<
            PersonDto,
            int,
            PersonGetAllRequestDto,
            CreatePersonDto,
            UpdatePersonDto
            >
    {
    }
}
