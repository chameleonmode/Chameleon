namespace Chameleon.Infrastructure.Api
{
    public class ApiResponseErrorDto
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public string ValidationErrors { get; set; }
    }
}
