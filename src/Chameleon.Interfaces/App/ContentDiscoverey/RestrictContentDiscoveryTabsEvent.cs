namespace Chameleon.Interfaces.App.ContentDiscoverey
{
    public class RestrictContentDiscoveryTabsEventArgs : EventArgs
    {
        public bool IsProspectorVisible { get; }
        public bool IsRssVisible { get; }
        public RestrictContentDiscoveryTabsEventArgs(
            bool isProspectorVisible,
            bool isRssVisible)
        {
            IsProspectorVisible = isProspectorVisible;
            IsRssVisible = isRssVisible;
        }
    }
    public class RestrictContentDiscoveryTabsEvent : PubSubEvent<RestrictContentDiscoveryTabsEventArgs>
    {
    }
}
