using System.ComponentModel;

namespace Chameleon.Interfaces.OutReach
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum OutReachLinkUrlType
    {
        [Description("Social")]
        Social,

        [Description("Blog")]
        Blog,

        [Description("Forum")]
        Forum,

        [Description("Comment")]
        Comment
    }
}
