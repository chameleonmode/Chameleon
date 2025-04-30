using Chameleon.AIR.Actors.Models;
using Chameleon.AIR.Actors.Models.Reddit;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation.Actors.ViewModels;
public partial class ArgsViewModel : ObservableObject {
	[ObservableProperty] string _search;
	[ObservableProperty] Scope _selectedScope;
	[ObservableProperty] Sort _selectedSort;
	[ObservableProperty] Filter _selectedFilter;

	public IEnumerable<Scope> AvailableScopes { get; } = Enum.GetValues<Scope>();
	public IEnumerable<Sort> AvailableSorts { get; } = Enum.GetValues<Sort>();
	public IEnumerable<Filter> AvailableFilters { get; } = Enum.GetValues<Filter>();

	public ArgsViewModel(DictionaryArgs sourceArgs) {
		Search =
			sourceArgs.TryGetValue("Search", out var search)
			? search?.ToString() ?? "" : "";
		SelectedScope =
			sourceArgs.TryGetValue("Scope", out var scope) && Enum.TryParse<Scope>(scope?.ToString(), out var scopeEnum)
			? scopeEnum : Scope.Posts;
		SelectedSort =
			sourceArgs.TryGetValue("Sort", out var sort) && Enum.TryParse<Sort>(sort?.ToString(), out var sortEnum)
			? sortEnum : Sort.Relevance;
		SelectedFilter =
			sourceArgs.TryGetValue("Filter", out var filter) && Enum.TryParse<Filter>(filter?.ToString(), out var filterEnum)
			? filterEnum : Filter.All;
	}

	public DictionaryArgs ToDictionary() {
		return new DictionaryArgs {
			["search"] = Search.Contains(',') ? Search.Split(",").Select(x => x.Trim()) : [Search.Trim()],
			["scope"] = SelectedScope.ToString(),
			["sort"] = SelectedSort.ToString(),
			["filter"] = SelectedFilter.ToString()
		};
	}
}
