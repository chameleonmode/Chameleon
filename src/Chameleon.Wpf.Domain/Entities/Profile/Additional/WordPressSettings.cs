using Chameleon.Interfaces.WordPress;

namespace Chameleon.Domain.Entities
{
    public class WordPressSettings : IWordPressSettings
    {
        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
