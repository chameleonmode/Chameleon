using Avalonia.Controls.Primitives;
using Chameleon.client.Features.Tenants.Members;
using Chameleon.client.UI.Fluent.Pages;
using FluentAvalonia.UI.Controls;

namespace Chameleon.client.Pages.Views;

[lib.Common.Attributes.ViewModel(typeof(Features.Tenants.TenantsViewModel))]
public partial class TenantsView : TabStripNavigationPage {
	public TenantsView() {
		InitializeComponent();
	}

	public override TabStrip Strip => ActiveTabStrip;
	public override Frame Frame => NavigationFrame;
	public override Type GetNavigationTarget(int index) => index switch {
		0 => typeof(TenantMembersView),
		_ => throw new Exception()
	};
}