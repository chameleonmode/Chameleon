using Chameleon.Interfaces.OutReach;
using System.ComponentModel;

namespace Chameleon.Interfaces.SocialAnimal
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Period
    {
        [Description("Year")]
        Year,

        [Description("6 Months")]
        SixMonths,

        [Description("3 Months")]
        ThreeMonths,

        [Description("Month")]
        Month,

        [Description("Week")]
        Week,

        [Description("24 Hours")]
        Day,

        [Description("Custom Date")]
        CustomDate
    }
}
