using System.Windows;
using System.Windows.Controls;

namespace Chameleon.Common.Extensions
{
    public static class ContentControlExtensions
    {
        /// <summary>
        /// set content to null first to force re render
        /// becasue sometimes content is not rendered
        /// </summary>
        /// <param name="self"></param>
        /// <param name="content"></param>
        public static void ResetContent(this ContentControl self, object content = null)
        {
            self.InvokeOnUiThread(() =>
            {
                self._SetContent(null);
                if (content != null)
                {
                    self._SetContent(content);
                }
            });
        }

        public static void ClearContent(this ContentControl self)
        {
            self.SetContent(null);
        }

        public static bool HasContent(this ContentControl self)
        {
            return self.Content != null;
        }

        public static bool HasNoContent(this ContentControl self)
        {
            return self.Content == null;
        }

        public static void SetContent(this ContentControl self, object content = null)
        {
            self.InvokeOnUiThread(() => self._SetContent(content));
        }

        private static void _SetContent(this ContentControl self, object content = null)
        {
            self.Content = content;
            if (content is FrameworkElement frameworkElement)
            {
                self.DataContext = frameworkElement.DataContext;
            }
        }
    }
}
