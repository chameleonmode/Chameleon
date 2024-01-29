namespace Chameleon.Interfaces.Breadcrumbs
{
    public interface IBreadcrumbViewModel
    {
        string Title { get; set; }
        bool IsBold { get; set; }
        bool HasContinuation { get; set; }
    }
}
