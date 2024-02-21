using System.ComponentModel.DataAnnotations;

namespace Chameleon.App
{
    public class WebBrowserUserAgentBaseDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        public bool IsDefault { get; set; }
    }
}
