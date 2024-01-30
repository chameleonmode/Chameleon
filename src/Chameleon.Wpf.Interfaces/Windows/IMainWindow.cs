using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;
using Microsoft.Maui.Controls;
using System.Windows;

namespace Chameleon.Interfaces.Windows
{
    public interface IMainWindow 
        : IViewContent
        , ISingletonDependency
    {
        Window GetWindow();
        void Show();
        void SetContent(object content, string title = "TEMP");
        void ShowWaitIndicator();
        void HideWaitIndicator();
    }
}
