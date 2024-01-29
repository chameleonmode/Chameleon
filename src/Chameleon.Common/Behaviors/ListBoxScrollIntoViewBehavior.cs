using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Chameleon.Common.Behaviors
{
    public class ListBoxScrollIntoViewBehavior : Behavior<ListBox>
    {
        /// <summary>
        ///  When Beahvior is attached
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        /// <summary>
        /// On Selection Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null)
            {
                return;
            }

            if (listBox.SelectedItem == null)
            {
                return;
            }

            listBox.Dispatcher.BeginInvoke((Action)(() =>
            {
                listBox.UpdateLayout();

                var selectedItem = listBox.SelectedItem;
                if (selectedItem != null)
                {
                    listBox.ScrollIntoView(selectedItem);
                }
            }));
        }
        /// <summary>
        /// When behavior is detached
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;

        }
    }
}
