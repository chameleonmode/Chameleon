using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;
using System;

namespace Chameleon.Interfaces.App.OutReach.OutReachLink
{
    public interface IReminderPopupView
        : IViewControl
        , Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        DateTime? ReminderDatetime { get; set; }
        string Notes { get; set; }
        bool IsSave { get; set; }
    }
}
