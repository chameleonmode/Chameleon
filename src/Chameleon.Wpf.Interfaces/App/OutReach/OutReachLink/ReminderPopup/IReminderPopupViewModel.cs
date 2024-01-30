using Chameleon.Interfaces.Ioc;
using System;

namespace Chameleon.Interfaces.App.OutReach.OutReachLink
{
    public interface IReminderPopupViewModel
        : ITransientDependency
    {
        DateTime? Date { set; get; }
        DateTime? Time { set; get; }
        string Notes { set; get; }
        bool IsShowDeleteButton { set; get; }
        DateTime? ReminderDatetime { set; get; }
        bool IsSave { set; get; }
    }
}
