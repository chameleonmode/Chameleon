using Chameleon.app.Avalonia.ViewModels.General;
using Chameleon.Av.Fluent.Common.Pages;

using FluentAvalonia.UI.Controls;

namespace Chameleon.app.Avalonia;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(AssistantTaskforceViewModel))]
public partial class AssistanTaskforceView : ChameleonPageBase {
	public AssistanTaskforceView()
	{
		InitializeComponent();
		ControlName = "Assistant Taskforce";
		Description = "Resource Management";
		PreviewImage = AppLayers.TryGetResource<IconSource>("User");
	}
}