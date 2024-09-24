using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Views;
using System.Windows;

namespace Chameleon.Interfaces.Windows
{
    public interface IMainWindow 
        : IViewContent
        , Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        //Window GetWindow();
        void Show();
        void SetContent(object content, string title = "TEMP");
        void SetContent(string content);
        void ShowWaitIndicator();
        void HideWaitIndicator();
    }
}
