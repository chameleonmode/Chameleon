using System.ComponentModel.DataAnnotations;

namespace Chameleon.App
{
    public class ProxyBaseDto
    {
        public string Host { get; set; }

        [Range(0, 65535)]
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
