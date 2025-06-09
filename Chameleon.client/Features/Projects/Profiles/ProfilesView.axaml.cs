using Chameleon.client.UI.Controls;

namespace Chameleon.client.Features.Projects.Profiles; 
[Chameleon.lib.Common.Attributes.ViewModel(typeof(ProfilesViewModel))]
public partial class ProfilesView : AutoViewModelInitControl {
    public ProfilesView() {
        InitializeComponent();
    }
}