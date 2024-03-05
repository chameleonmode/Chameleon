using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.Paginator
{
    public interface IPaginatorViewModel : ITransientDependency
    {
        int PageIndex { get; set; }
        int PageCount { get; }
        int TotalCount { get; set; }
        int Skip { get; }
        int OnPageItems { get; }
    }
}
