using Chameleon.Av.Fluent.Common.Controls;

namespace Chameleon.client.Features.Projects.Profiles.Dialogs;

public partial class SnapCracklePopUserControl : AutoViewModelInitControl {
    public SnapCracklePopUserControl() {
        InitializeComponent();
    }
    public static SnapCracklePopUserControl Instance { get; } = new SnapCracklePopUserControl();
}