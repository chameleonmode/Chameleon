using Chameleon.Av.Fluent.Common.Pages;
using FluentAvalonia.UI.Controls;

namespace Chameleon.client.Features.Automation.Actors;
public partial class ActorsView : ChameleonPageBase {
	public ActorsView()
	{
		InitializeComponent();
		ControlName = "Mr. Roboto";
		Description = "AI Robot Agents & Automationed Actors";
		PreviewImage = App.TryGetResource<IconSource>("SpiderIcon");
	}
}