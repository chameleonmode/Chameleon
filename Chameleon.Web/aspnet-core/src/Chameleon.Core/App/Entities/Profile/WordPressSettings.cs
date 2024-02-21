using Microsoft.EntityFrameworkCore;

namespace Chameleon.App.Entities
{
    [Owned]
    public class WordPressSettings
    {
        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
