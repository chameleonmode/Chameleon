using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Chameleon.Common.Behaviors
{
    public class TextBoxDoubleClickBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.MouseDoubleClick += AssociatedObjectMouseDoubleClick;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseDoubleClick -= AssociatedObjectMouseDoubleClick;
            base.OnDetaching();
        }

        private void AssociatedObjectMouseDoubleClick(object sender, RoutedEventArgs routedEventArgs)
        {
            AssociatedObject.SelectAll();
        }
    }
}
