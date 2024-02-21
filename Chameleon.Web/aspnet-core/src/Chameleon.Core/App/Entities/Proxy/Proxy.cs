using Abp.Domain.Entities.Auditing;

namespace Chameleon.App.Entities
{
    public class Proxy : FullAuditedEntity
    {
        public string Host { get; set; }

        private int _port;
        public int Port
        {
            get => _port;
            set 
            { 
                if (value < 0)
                {
                    // TODO: add validation
                }
                _port = value;
            }
        }

        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
