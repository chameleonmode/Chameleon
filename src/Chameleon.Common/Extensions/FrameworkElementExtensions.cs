using System.Windows;
using System.Windows.Data;

namespace Chameleon.Common.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static void SetBinding(this FrameworkElement self, 
            DependencyProperty dp, string path, BindingMode bindingMode)
        {
            self.SetBinding(dp, new Binding(path)
            {
                Mode = bindingMode
            });
        }
    }
}
