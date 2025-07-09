using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Chameleon.client.Features.Automation;
using Chameleon.client.Features.Projects.Profiles;
using Chameleon.lib.Api.Repos;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using DynamicData.Binding;
namespace Chameleon.client.Features.Projects;

public enum ProfileUIModule { Profiles, Favourites, Tags }
public enum ProfileUIContext { Profiles, Dashboard, Favorites, Automation, Dialog, Identity }

public interface IProfileUIContextAware {
  ProfileUIContext CurrentContext { get; }
  ProfileUIContext? PreviousContext { get; }
  void SetUIContext(ProfileUIContext context);
}

public record ProfileUIState(bool IsShowCheckboxColumn = true, bool IsShowGlyph = true, bool IsActionOptionsVisible = true, bool IsSelectionEnabled = true);

public static class ProfileUIStateMachine {
  private static readonly Dictionary<ProfileUIContext, ProfileUIState> States = new() {
    [ProfileUIContext.Profiles] = new(
       IsShowCheckboxColumn: true,
       IsShowGlyph: true,
       IsActionOptionsVisible: true,
       IsSelectionEnabled: true
    ),
    [ProfileUIContext.Identity] = new(
       IsShowCheckboxColumn: false,
       IsShowGlyph: true,
       IsActionOptionsVisible: true,
       IsSelectionEnabled: true
    ),
    [ProfileUIContext.Dashboard] = new(
       IsShowCheckboxColumn: false,
       IsShowGlyph: true,
       IsActionOptionsVisible: true,
       IsSelectionEnabled: false
    ),
    [ProfileUIContext.Favorites] = new(
       IsShowCheckboxColumn: false,
       IsShowGlyph: true,
       IsActionOptionsVisible: true,
       IsSelectionEnabled: false
    ),
    [ProfileUIContext.Automation] = new(
       IsShowCheckboxColumn: true,
       IsShowGlyph: false,
       IsActionOptionsVisible: false,
       IsSelectionEnabled: true
    ),
    [ProfileUIContext.Dialog] = new(
       IsShowCheckboxColumn: true,
       IsShowGlyph: false,
       IsActionOptionsVisible: false,
       IsSelectionEnabled: true
    )
  };

  public static ProfileUIState GetStateFor(ProfileUIContext context) {
    return States.TryGetValue(context, out var state)
        ? state
        : States[ProfileUIContext.Profiles];
  }

  public static bool CanTransition(ProfileUIContext from, ProfileUIContext to) {
    // For now, allow all transitions - can be restricted later if needed
    return true;
  }

  public static IEnumerable<ProfileUIContext> GetAllContexts() {
    return States.Keys;
  }
}
public static class ProfileUIContextManager {
  private static readonly SemaphoreSlim Semaphore = new(1, 1);
  private static readonly Dictionary<ProfileUIModule, ProfileUIContext> ModuleContexts = [];

  static async Task<T> Locker<T>(Func<Task<T>> action) {
    await Semaphore.WaitAsync();
    try {
      return await action();
    } finally {
      _ = Semaphore.Release();
    }
  }
  static async Task<T> Locker<T>(Func<T> action) => await Locker(() => {
    return Task.FromResult(action());
  });
  static async Task Locker(Action action) => await Locker(() => {
    action();
    return true;
  });

  public static async void SetModuleContext(ProfileUIModule moduleId, ProfileUIContext context) => await Locker(() => {
    var current = ModuleContexts.GetValueOrDefault(moduleId, ProfileUIContext.Profiles);
    ModuleContexts[moduleId] = ProfileUIStateMachine.CanTransition(current, context)
      ? context
      : throw new InvalidOperationException($"Cannot transition from {current} to {context}");
  });

  public static async Task<ProfileUIContext> GetCurrentContext(ProfileUIModule moduleId) => await Locker(() => {
    return ModuleContexts.GetValueOrDefault(moduleId, ProfileUIContext.Profiles);
  });

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
    profile.IsActionOptionsVisible = state.IsActionOptionsVisible;
    profile.IsSelectionEnabled = state.IsSelectionEnabled;

    if (profile is IProfileUIContextAware contextAware) {
      contextAware.SetUIContext(context);
    }
  }

  public static async Task<bool> ClearModuleContext(ProfileUIModule moduleId) => await Locker(() => {
    return ModuleContexts.Remove(moduleId);
  });

  public static async Task<Dictionary<ProfileUIModule, ProfileUIContext>> GetAllActiveContextsAsync() => await Locker(() => {
    return new Dictionary<ProfileUIModule, ProfileUIContext>(ModuleContexts);
  });
}

public abstract partial class Profilearee(string? title = null) : Automatior(title) {
  [ObservableProperty] ChangeComparereOption sort = ChangeComparereOption.Ascending;
  public abstract ReadOnlyObservableCollection<ObsProfile> Profiles { get; }
  public ChangeComparereOption[] Sorts { get; } = (ChangeComparereOption[])Enum.GetValues(typeof(ChangeComparereOption));
  public bool HasProfiles => Profiles.Count > 0;

  partial void OnSortChanged(ChangeComparereOption value) {
    Profiler.CompareObservable.OnNext(value switch {
      ChangeComparereOption.Descending => Profiler.DescendingComparer,
      _ => Profiler.AscendingComparer
    });
  }
}
public abstract partial class Profiler : Profilearee {
  public static readonly SortExpressionComparer<ObsProfile> AscendingComparer = SortExpressionComparer<ObsProfile>.Ascending(p => p.Dto.title ?? "");
  public static readonly SortExpressionComparer<ObsProfile> DescendingComparer = SortExpressionComparer<ObsProfile>.Descending(p => p.Dto.title ?? "");
  public static readonly BehaviorSubject<IComparer<ObsProfile>> CompareObservable = new(AscendingComparer);
  public IObservable<IChangeSet<ObsProfile, int>> Shared { get; }
  public ReadOnlyObservableCollection<ObsProfile> ObsProfiles { get; }
  public virtual IEnumerable<ObsProfile> SelectedProfiles => Profiles.Where(i => i.IsSelected);
  public virtual int SelectedCount => SelectedProfiles.Count();
  public virtual bool HasSelectedItems => SelectedProfiles.Any();

  public Profiler(string? title = null) : base(title) {
    // 1) Create a shared change‐set (after your Transform + Filter)
    Shared = UserProfilesRepo.Connect().Transform(i => new ObsProfile(i,
      selectedChanged: SelectedChanged,
      onDeleted: p => Deleted(p)))
    .Publish()    // <-- multicast
    .RefCount();  // <-- auto-connect when first subscriber appears

    // 3) Your “full” (un-paged) view
    _ = Shared
    .SortAndBind(out var allProfiles, AscendingComparer)
    .Subscribe();

    // 4) Expose both lists
    ObsProfiles = allProfiles;
  }
  public virtual void SelectedChanged(ObsProfile _) {
    RefreshProperties();
  }
  public virtual void RefreshProperties() {
		OnPropertyChanged(nameof(HasProfiles));
		OnPropertyChanged(nameof(SelectedCount));
		OnPropertyChanged(nameof(HasSelectedItems));
	}

  public virtual ObsProfile Deleted(ObsProfile profile) {
    return profile;
  }
}