using Chameleon.Interfaces.OutReach;
using System;

namespace Chameleon.Domain.Entities
{
    public class ProfileOutReachLink 
        : IProfileOutReachLink
    {
        public int Id { get; set; }
        public string UrlName { get; set; }
        public string Url { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string Notes { get; set; }
        public OutReachLinkStatus Status { get; set; }
        public OutReachLinkUrlType UrlType { get; set; }
        public int ProfileId { get; set; }
        public string Twitter { get; set; }
        public string Linkedin { get; set; }
        public string Facebook { get; set; }
        public string OtherSocial { get; set; }
        public string ReminderNotes { get; set; }
        public DateTime? ReminderDatetime { get; set; }
        public bool IsEnable { get; set; }
    }
}
