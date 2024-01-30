namespace Chameleon.Interfaces.Views
{
    public interface IViewContent
    {
        object GetContent();
        void SetContent(INavigationContent navigationContent);
    }
}
