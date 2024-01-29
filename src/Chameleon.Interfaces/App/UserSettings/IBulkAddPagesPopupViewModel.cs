using Chameleon.Interfaces.Ioc;
using System;

namespace Chameleon.Interfaces.App.UserSettings
{
    public interface IBulkAddPagesPopupViewModel
        : ITransientDependency
    {
        string Urls { set; get; }
    }
}
