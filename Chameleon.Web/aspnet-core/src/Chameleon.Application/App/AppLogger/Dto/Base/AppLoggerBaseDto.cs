using Chameleon.App.Entities;
using System;

namespace Chameleon.App
{
    public class AppLoggerBaseDto
    {
        public string Message { get; set; }
        public string AppLoggerType { get; set; }
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
