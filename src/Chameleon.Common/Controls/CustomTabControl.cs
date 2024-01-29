using System.Windows;
using System.Windows.Controls;

namespace Chameleon.Common.Controls
{
    public class CustomTabControl : TabControl
    {
        private const string TabControlStyleKey = "MainTabControl";
        private const string TabItemStyleKey = "MainTabItem";

        public CustomTabControl()
            : base()
        {
            Initialized += CustomTabControl_Initialized;
        }

        private void CustomTabControl_Initialized(object sender, System.EventArgs e)
        {
            SetStyles();
        }

        private void SetStyles()
        {
            if(Style == null)
            {
                Style = FindStyle(TabControlStyleKey);
            }
            
            foreach (TabItem item in Items)
            {
                if(item.Style == null)
                {
                    item.Style = FindStyle(TabItemStyleKey);
                }                
            }
        }

        private Style FindStyle(string key)
        {
            return (Style)FindResource(key);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            var width = (ActualWidth / Items.Count);
            width = width < 0 ? 0 : width;
            for (var i = 0; i < Items.Count; ++i)
            {
                var item = (TabItem)Items[i];                
                if(i == 0)
                {
                    item.Width = width - 1;
                    continue;
                }
                item.Width = width;
            }
        }
    }
}
