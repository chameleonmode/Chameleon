using Prism.Commands;

namespace Chameleon.Interfaces.DialogWindows
{
    public interface IDiscardSaveButtonsViewModel
    {
        DelegateCommand DiscardCommand { get; }
        DelegateCommand SaveChangesCommand { get; }
    }
}
