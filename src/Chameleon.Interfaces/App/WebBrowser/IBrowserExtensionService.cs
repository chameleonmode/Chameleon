using Chameleon.Interfaces.Ioc;
using System;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface IBrowserExtensionService
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
        , IDisposable
    {
        void Start();
    }
}
