using Chameleon.Infrastructure.Dto;

namespace Chameleon.Infrastructure.WebBrowsers
{
    public class WebBrowserUserAgentDto 
        : IEntityDto<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}