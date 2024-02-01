using Chameleon.Interfaces.App.ChangeLog;

namespace Chameleon.Domain.Entities.ChangeLog
{
    public class ChangeLog
       : IChangeLog
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
