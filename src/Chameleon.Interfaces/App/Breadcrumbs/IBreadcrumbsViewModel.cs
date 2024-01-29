using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Breadcrumbs
{
    public interface IBreadcrumbsViewModel : ITransientDependency
    {
        IBreadcrumbViewModel Root { get; }
    }
}
