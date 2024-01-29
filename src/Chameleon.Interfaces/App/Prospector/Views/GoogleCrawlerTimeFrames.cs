using Chameleon.Interfaces.OutReach;
using System.ComponentModel;

namespace Chameleon.Interfaces.Prospector
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum GoogleCrawlerTimeFrames
    {
        [Description("Any time")]
        AnyTime,

        [Description("Past hour")]
        PastHour,

        [Description("Past 24 hours")]
        Past24Hours,

        [Description("Past week")]
        PastWeek,

        [Description("Past month")]
        PastMonth,

        [Description("Past year")]
        PastYear        
    }
}
