using System.ComponentModel;

namespace Chameleon.Interfaces.OutReach
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum OutReachLinkStatus
    {
        [Description("Live")]
        Live,

        [Description("Not Live")]
        NotLive,

        [Description("Email Sent")]
        EmailSent
    }
}
