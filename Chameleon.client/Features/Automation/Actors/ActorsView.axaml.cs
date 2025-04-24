using Chameleon.Av.Fluent.Common.Pages;
using FluentAvalonia.UI.Controls;

namespace Chameleon.client.Features.Automation.Actors;
public partial class ActorsView : ChameleonPageBase {
	public ActorsView()
	{
		InitializeComponent();
		ControlName = "Chameleon AIR";
		Description = "Automationed Intelligence Respondent";
		PreviewImage = App.TryGetResource<IconSource>("SpiderIcon");
	}
}