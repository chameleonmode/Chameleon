using System;

namespace Chameleon.Interfaces.App.OutReach.Events.ReminderDate
{
    public class SetReminderDateEventArgs : EventArgs
    {
        public DateTime Date { get; }

        public SetReminderDateEventArgs(DateTime date)
        {
            Date = date.Date;
        }
    }
}
