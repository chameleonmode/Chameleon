using Chameleon.Interfaces.Ioc;
using System.Collections.Generic;

namespace Chameleon.Controls.ImportExport.Models
{
    public interface IImportColumnViewModels : 
        Chameleon.lib.Common.Interfaces.Systemics.ITransientDependency
    {
        IReadOnlyList<IImportColumnViewModel> Selected { get; }
    }    
}
