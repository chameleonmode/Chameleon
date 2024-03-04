using Chameleon.Interfaces.Views;

namespace Chameleon.Interfaces.SidePanel
{
    public class ShowSidePanelEvent : PubSubEvent<SidePanelEventArgs>
    {
    }

    public class SidePanelEventArgs : EventArgs
    {
        public string MainHeader { get; set; }
        public string SecondaryHeader { get; set; }
        public IViewControl ViewControl { get; set; }

        public SidePanelEventArgs(string mainHeader, string secondaryHeader, IViewControl viewControl)
        {
            MainHeader = mainHeader;
            SecondaryHeader = secondaryHeader;
            ViewControl = viewControl;

        }
    }
}
