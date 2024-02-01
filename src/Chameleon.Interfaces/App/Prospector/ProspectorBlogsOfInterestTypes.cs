using Chameleon.Interfaces.OutReach;
using System.ComponentModel;

namespace Chameleon.Interfaces.App.Prospector
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ProspectorBlogsOfInterestTypes
    {
        [Description("Comment backlinks")]
        CommentBacklinks,

        [Description("Forum")]
        Forum,

        [Description("Guest posts")]
        GuestPosts,

        [Description("Blog")]
        Blog,

        [Description("Link roundups")]
        LinkRoundups
    }
}
