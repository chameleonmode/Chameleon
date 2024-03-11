using Chameleon.Interfaces.Ioc;
using System.Collections.Generic;

namespace Chameleon.Controls.ImportExport.Models
{
    public interface IImportColumnViewModels : 
        ITransientDependency
    {
        IReadOnlyList<IImportColumnViewModel> Selected { get; }
    }    
}
