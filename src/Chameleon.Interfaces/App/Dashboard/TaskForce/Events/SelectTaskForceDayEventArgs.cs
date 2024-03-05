using System;

namespace Chameleon.Interfaces.App.Dashboard.TaskForce.Events
{
    public class SelectTaskForceDayEventArgs : EventArgs
    {
        public DateTime SelectedDateTime { get; }
        public SelectTaskForceDayEventArgs(DateTime day)
        {
            SelectedDateTime = day;
        }
    }
}
