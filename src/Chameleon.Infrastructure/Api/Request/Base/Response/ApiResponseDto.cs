namespace Chameleon.Infrastructure.Api
{
    public class ApiResponseDto
    {
        public object Result { get; set; }
        public bool Success { get; set; }
        public ApiResponseErrorDto Error { get; set; }
    }
}
