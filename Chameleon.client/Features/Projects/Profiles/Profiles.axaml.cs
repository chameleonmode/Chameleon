using Chameleon.client.UI.Controls;

namespace Chameleon.client.Features.Projects.Profiles;
public partial class ProfilesView : AutoViewModelLocatorControl {
  public ProfilesView() {
    InitializeComponent();
  }

  protected override object? ViewModel => ProfilesViewModel.Instance;
}