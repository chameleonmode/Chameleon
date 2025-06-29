namespace Chameleon.client.Features.Projects.Profiles;

public interface IProfileUIContextAware {
	void SetUIContext(ProfileUIContext context);
	ProfileUIContext GetUIContext();
}

public static class ProfileUIContextManager {
	private static readonly Dictionary<string, ProfileUIContext> ModuleContexts = new();
	private static readonly object LockObject = new();

	public static void SetModuleContext(string moduleId, ProfileUIContext context) {
		lock (LockObject) {
			var previousContext = ModuleContexts.GetValueOrDefault(moduleId, ProfileUIContext.ProfilesView);

			ModuleContexts[moduleId] = ProfileUIStateMachine.CanTransition(previousContext, context)
				? context
				: throw new InvalidOperationException($"Cannot transition from {previousContext} to {context}");
		}
	}

	public static ProfileUIContext GetCurrentContext(string moduleId) {
		lock (LockObject) {
			return ModuleContexts.GetValueOrDefault(moduleId, ProfileUIContext.ProfilesView);
		}
	}

	public static void ApplyContextToProfiles(IEnumerable<ObsProfile> profiles, ProfileUIContext context) {
		var state = ProfileUIStateMachine.GetStateFor(context);
		foreach (var profile in profiles) {
			ApplyStateToProfile(profile, state, context);
		}
	}

	public static void ApplyContextToProfile(ObsProfile profile, ProfileUIContext context) {
		ApplyContextToProfiles(new[] { profile }, context);
	}

	private static void ApplyStateToProfile(ObsProfile profile, ProfileUIState state, ProfileUIContext context) {
		profile.IsShowCheckboxColumn = state.IsShowCheckboxColumn;
		profile.IsShowGlyph = state.IsShowGlyph;

		if (profile is IProfileUIContextAware contextAware) {
			contextAware.SetUIContext(context);
		}
	}

	public static void ClearModuleContext(string moduleId) {
		lock (LockObject) {
			ModuleContexts.Remove(moduleId);
		}
	}

	public static Dictionary<string, ProfileUIContext> GetAllActiveContexts() {
		lock (LockObject) {
			return new Dictionary<string, ProfileUIContext>(ModuleContexts);
		}
	}
}
