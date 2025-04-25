using Chameleon.AIR.Actors.Models.Reddit;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Automation.Actors.ViewModels;
public partial class ArgsViewModel: ObservableObject {
	[ObservableProperty] string? _search;
	[ObservableProperty] Scope _selectedScope;
	[ObservableProperty] Sort _selectedSort;
	[ObservableProperty] Filter _selectedFilter;

	public Dictionary<string, object> OtherArgs { get; } = new();

	public IEnumerable<Scope> AvailableScopes => Enum.GetValues<Scope>();
	public IEnumerable<Sort> AvailableSorts => Enum.GetValues<Sort>();
	public IEnumerable<Filter> AvailableFilters => Enum.GetValues<Filter>();

	public ArgsViewModel() { }

	public ArgsViewModel(IDictionary<string, object> sourceArgs) {

		Search = sourceArgs.TryGetValue("Search", out var search) ? search?.ToString() : null;
		SelectedScope = sourceArgs.TryGetValue("Scope", out var scope) && scope is Scope scopeEnum ? scopeEnum : Scope.Posts;
		SelectedSort = sourceArgs.TryGetValue("Sort", out var sort) && sort is Sort sortEnum ? sortEnum : Sort.Relevance;
		SelectedFilter = sourceArgs.TryGetValue("Filter", out var filter) && filter is Filter filterEnum ? filterEnum : Filter.All;


		OtherArgs.Clear();
		var handledKeys = new HashSet<string> { "Search", "Scope", "Sort", "Filter" };
		foreach (var kvp in sourceArgs) {
			if (!handledKeys.Contains(kvp.Key)) {
				OtherArgs.Add(kvp.Key, kvp.Value);
			}
		}
	}

	public Dictionary<string, object> ToDictionary() {
		var dict = new Dictionary<string, object>(OtherArgs) {
			["Search"] = Search ?? "",
			["Scope"] = SelectedScope,
			["Sort"] = SelectedSort,
			["Filter"] = SelectedFilter
		};
		return dict;
	}
}
