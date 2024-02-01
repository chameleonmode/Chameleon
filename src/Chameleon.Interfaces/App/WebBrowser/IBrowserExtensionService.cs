using Chameleon.Interfaces.Ioc;
using System;

namespace Chameleon.Interfaces.WebBrowser
{
    public interface IBrowserExtensionService
        : ISingletonDependency
        , IDisposable
    {
        void Start();
    }
}
