using Chameleon.Av.Fluent.Common.Controls;

namespace Chameleon.client.Features.Projects.Profiles.MyProfiles; 
[Chameleon.lib.Common.Attributes.ViewModel(typeof(ProfilesViewModel))]
public partial class MyProfilesView : AutoViewModelInitControl {
    public MyProfilesView() {
        InitializeComponent();
    }
}