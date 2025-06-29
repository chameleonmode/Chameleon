namespace Chameleon.client.Features.Projects.Profiles;

public enum ProfileUIContext {
    ProfilesView,
    DashboardTags,
    DashboardFavorites,
    AutomationSelection,
    DialogSelection
}


public class ProfileUIState(bool showCheckbox = true, bool showGlyph = true, bool actionOptions = true, bool selectionEnabled = true) {
	public bool IsShowCheckboxColumn { get; set; } = showCheckbox;
	public bool IsShowGlyph { get; set; } = showGlyph;
	public bool IsActionOptionsVisible { get; set; } = actionOptions;
	public bool IsSelectionEnabled { get; set; } = selectionEnabled;
}

public static class ProfileUIStateMachine {
    private static readonly Dictionary<ProfileUIContext, ProfileUIState> States = new() {
        [ProfileUIContext.ProfilesView] = new(
            showCheckbox: true,
            showGlyph: true, 
            actionOptions: true,
            selectionEnabled: true
        ),
        [ProfileUIContext.DashboardTags] = new(
            showCheckbox: false,
            showGlyph: true,
            actionOptions: false,
            selectionEnabled: false
        ),
        [ProfileUIContext.DashboardFavorites] = new(
            showCheckbox: false,
            showGlyph: true,
            actionOptions: false,
            selectionEnabled: false
        ),
        [ProfileUIContext.AutomationSelection] = new(
            showCheckbox: true,
            showGlyph: false,
            actionOptions: false,
            selectionEnabled: true
        ),
        [ProfileUIContext.DialogSelection] = new(
            showCheckbox: true,
            showGlyph: false,
            actionOptions: false,
            selectionEnabled: true
        )
    };

    public static ProfileUIState GetStateFor(ProfileUIContext context) {
        return States.TryGetValue(context, out var state) 
            ? state 
            : States[ProfileUIContext.ProfilesView];
    }

    public static bool CanTransition(ProfileUIContext from, ProfileUIContext to) {
        // For now, allow all transitions - can be restricted later if needed
        return true;
    }

    public static IEnumerable<ProfileUIContext> GetAllContexts() {
        return States.Keys;
    }
}
