namespace Chameleon.Interfaces.Views
{
    public interface IViewContentContext
    {
        string Title { get; }
        object GetContext();
        void SetContext(object context);
    }
}
