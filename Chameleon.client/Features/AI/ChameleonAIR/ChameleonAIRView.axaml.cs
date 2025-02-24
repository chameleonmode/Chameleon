using Chameleon.Av.Fluent.Common.Pages;

using FluentAvalonia.UI.Controls;

namespace Chameleon.client.Features.AI.ChameleonAIR;
public partial class ChameleonAIRView : ChameleonPageBase {
	public ChameleonAIRView()
	{
		InitializeComponent();
		ControlName = "Chameleon AIR";
		Description = "Automationed Intelligence Respondent";
		PreviewImage = App.TryGetResource<IconSource>("AutomationIcon2");
	}
}