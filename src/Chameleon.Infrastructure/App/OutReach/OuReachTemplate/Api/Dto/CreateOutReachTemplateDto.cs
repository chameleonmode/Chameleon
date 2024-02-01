namespace Chameleon.Infrastructure.OutReach.Api.Dto
{
    public class CreateOutReachTemplateDto
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string Subject { get; set; }
    }
}
