using Chameleon.lib.CommunityToolkit.MvvM;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Helpers;

namespace Chameleon.client.Features.Automation.Actors.Dialogs;

public partial class ScriptsSelectorViewModel(List<Selection> selections) : ViewModelObjectBase("Select Automations") {
	public List<Selection> AvailableSelections { get; } = selections;

	public IEnumerable<Selection> SelectedScripts => AvailableSelections.Where(s => s.Selected);

	public async Task<bool> ShowDialogAsync() {
		var result = await MessageBox.ShowTaskDialog<ScriptsSelectorView, ScriptsSelectorViewModel>(new(
				Initialize: () => this, 
				Header: "Select Automations to Run",
				SubHeader: "Choose one or more automations to execute.",
				Symbas: Symbas.Setting,
				Btns: MBoxButtons.OkCancel)
		);
		return result == TaskDialogResult.OK && SelectedScripts.Any();
	}
}