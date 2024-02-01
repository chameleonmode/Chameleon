using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.ExceptionOptions
{
    public class AppLoggerDto
        : AppLoggerBaseDto
        , IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
