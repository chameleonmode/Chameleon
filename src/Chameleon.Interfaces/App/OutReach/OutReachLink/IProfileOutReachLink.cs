using Chameleon.Interfaces.UserProfiles;
using System;

namespace Chameleon.Interfaces.OutReach
{
    public interface IProfileOutReachLink
        : IUserProfileRequiredEntity
    {
        string UrlName { get; set; }
        string Url { get; set; }
        string ContactName { get; set; }
        string ContactEmail { get; set; }
        string Notes { get; set; }
        OutReachLinkStatus Status { get; set; }
        OutReachLinkUrlType UrlType { get; set; }
        string Twitter { get; set; }
        string Linkedin { get; set; }
        string Facebook { get; set; }
        string OtherSocial { get; set; }
        string ReminderNotes { get; set; }
        DateTime? ReminderDatetime { get; set; }
        bool IsEnable { get; set; }
    }
}
