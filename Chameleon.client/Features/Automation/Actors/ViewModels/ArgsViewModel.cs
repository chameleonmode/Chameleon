using Chameleon.AIR.Actors.Models.Reddit;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation.Actors.ViewModels;
public partial class ArgsViewModel : ObservableObject {
	[ObservableProperty] string? _search;
	[ObservableProperty] Scope _selectedScope;
	[ObservableProperty] Sort _selectedSort;
	[ObservableProperty] Filter _selectedFilter;

	public IEnumerable<Scope> AvailableScopes => Enum.GetValues<Scope>();
	public IEnumerable<Sort> AvailableSorts => Enum.GetValues<Sort>();
	public IEnumerable<Filter> AvailableFilters => Enum.GetValues<Filter>();

	public ArgsViewModel(IDictionary<string, string> sourceArgs) {
		Search = sourceArgs.TryGetValue("Search", out var search) ? search?.ToString() : null;
		SelectedScope =
			sourceArgs.TryGetValue("Scope", out var scope) && Enum.TryParse<Scope>(scope, out var scopeEnum)
			? scopeEnum : Scope.Posts;
		SelectedSort =
			sourceArgs.TryGetValue("Sort", out var sort) && Enum.TryParse<Sort>(sort, out var sortEnum)
			? sortEnum : Sort.Relevance;
		SelectedFilter =
			sourceArgs.TryGetValue("Filter", out var filter) && Enum.TryParse<Filter>(filter, out var filterEnum)
			? filterEnum : Filter.All;
	}

	public Dictionary<string, string> ToDictionary() {
		return new Dictionary<string, string> {
			["search"] = Search ?? "",
			["scope"] = SelectedScope.ToString(),
			["sort"] = SelectedSort.ToString(),
			["filter"] = SelectedFilter.ToString()
		};
	}
}
