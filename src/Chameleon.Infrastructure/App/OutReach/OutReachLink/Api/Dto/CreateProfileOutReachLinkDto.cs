using System;
using System.Collections.Generic;
using System.Text;

namespace Chameleon.Infrastructure.OutReachLink
{
    public class CreateProfileOutReachLinkDto
    {
        public string UrlName { get; set; }
        public string Url { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public string UrlType { get; set; }
        public int ProfileId { get; set; }
        public string Twitter { get; set; }
        public string Linkedin { get; set; }
        public string Facebook { get; set; }
        public string OtherSocial { get; set; }
        public string ReminderNotes { get; set; }
        public DateTime? ReminderDatetime { get; set; }
    }
}
