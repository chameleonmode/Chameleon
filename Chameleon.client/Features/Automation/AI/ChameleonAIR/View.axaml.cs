using Chameleon.Av.Fluent.Common.Pages;
using FluentAvalonia.UI.Controls;

namespace Chameleon.client.Features.Automation.AI.ChameleonAIR;
public partial class View : ChameleonPageBase {
	public View()
	{
		InitializeComponent();
		ControlName = "Chameleon AIR";
		Description = "Automationed Intelligence Respondent";
		PreviewImage = App.TryGetResource<IconSource>("AutomationIcon2");
	}
}