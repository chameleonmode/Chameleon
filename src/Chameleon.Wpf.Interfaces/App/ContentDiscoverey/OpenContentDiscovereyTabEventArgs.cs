using System;

namespace Chameleon.Interfaces.App.ContentDiscoverey
{
    public class OpenContentDiscovereyTabEventArgs : EventArgs
    {
        public ContentDiscovereyTabs ContentDiscovereyTab { get;}

        public OpenContentDiscovereyTabEventArgs(ContentDiscovereyTabs contentDiscovereyTab)
        {
            ContentDiscovereyTab = contentDiscovereyTab;
        }
    }
}
