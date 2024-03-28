using Chameleon.Interfaces;
using Chameleon.Interfaces.Dialogs.ViewModels;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Controls.AssistantUsers.Interfaces
{
    public interface IInviteUserOrAddProfilesViewModel 
        : IContentDialogViewModel,
        ITransientDependency
    {
        bool ShowInviteinfo { get; set; }

        string TitleText { get; set; }

        string SaveChangesBtnText { get; set; }

        long AssistantId { get; set; }
    }
}
