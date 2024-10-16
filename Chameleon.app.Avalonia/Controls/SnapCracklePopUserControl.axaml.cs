using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Chameleon.Av.Fluent.Common.Controls;

namespace Chameleon.app.Avalonia.Controls;

public partial class SnapCracklePopUserControl : AutoViewModelInitControl {
    public SnapCracklePopUserControl()
    {
        InitializeComponent();
    }
	public static SnapCracklePopUserControl Instance { get; } = new SnapCracklePopUserControl();
}