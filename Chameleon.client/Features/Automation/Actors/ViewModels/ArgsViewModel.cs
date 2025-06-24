using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.Json;
using Chameleon.lib.AIR.Actors;
using Chameleon.lib.AIR.Actors.Reddit;

namespace Chameleon.client.Features.Automation.Actors.ViewModels;
public partial class ArgsViewModel : ObservableObject {
	[ObservableProperty] string search = "";
	[ObservableProperty] Scope selectedScope;
	[ObservableProperty] Sort selectedSort;
	[ObservableProperty] Filter selectedFilter;

	public IEnumerable<Scope> AvailableScopes { get; } = Enum.GetValues<Scope>();
	public IEnumerable<Sort> AvailableSorts { get; } = Enum.GetValues<Sort>();
	public IEnumerable<Filter> AvailableFilters { get; } = Enum.GetValues<Filter>();

	public void Set(DictionaryArgs sourceArgs) {
		Search = GetValue(sourceArgs, "Search", string.Empty) ?? string.Empty;
		SelectedScope = GetValue(sourceArgs, "Scope", Scope.Posts);
		SelectedSort = GetValue(sourceArgs, "Sort", Sort.Relevance);
		SelectedFilter = GetValue(sourceArgs, "Filter", Filter.All);
	}

	public DictionaryArgs ToDictionary(IEnumerable<Selection> selections, IEnumerable<string> terms) {
		return new DictionaryArgs {
			["search"] = terms,
			["scope"] = SelectedScope.ToString(),
			["sort"] = SelectedSort.ToString(),
			["filter"] = SelectedFilter.ToString(),
			["artifacters"] = new List<Artifact>() {
				new() {
					["type"] = "selections",
					["data"] = selections.Select(x => x.Script.Title.ToLower())
				}
			}
		};
	}

	private static T? GetValue<T>(DictionaryArgs dictionary, string key,
		T? defaultValue = default, JsonSerializerOptions? options = null) {

		options ??= new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };
		key = key.ToLowerInvariant();
		if (dictionary.TryGetValue(key, out var value)) {
			if (value == null) {
				return defaultValue;
			}

			if (value is T directValue) {
				return directValue;
			}

			if (value is JsonElement jsonElement) {
				try {
					if (key.Equals("Search", StringComparison.OrdinalIgnoreCase) &&
							typeof(T) == typeof(string) &&
							jsonElement.ValueKind == JsonValueKind.Array) {
						Debug.WriteLine($"Detected array for key '{key}'. Attempting to extract first string element.");
						var arrayEnumerator = jsonElement.EnumerateArray();
						if (arrayEnumerator.MoveNext() && arrayEnumerator.Current.ValueKind == JsonValueKind.String) {
							var stringValues = new List<string>();
							do {
								if (arrayEnumerator.Current.ValueKind == JsonValueKind.String && arrayEnumerator.Current.GetString()?.Trim() is string term) {
									stringValues.Add(term);
								}
							} while (arrayEnumerator.MoveNext());
							return (T)(object)string.Join(", ", stringValues);
						} else {
							Debug.WriteLine($"'{key}' key had unexpected array format or non-string element: {jsonElement.GetRawText()}");
							return defaultValue;
						}
					}
					return jsonElement.Deserialize<T>(options);
				} catch (NotSupportedException nsex) {
					Debug.WriteLine($"Type '{typeof(T).Name}' might not be directly supported for deserialization from JsonElement " +
						$"for key '{key}'. Error: {nsex.Message}");
					return typeof(T) == typeof(int) && jsonElement.TryGetInt32(out var intVal) ? (T)(object)intVal : defaultValue;
				} catch (JsonException jsonEx) {
					Debug.WriteLine($"Failed to deserialize JsonElement for key '{key}' to type '{typeof(T).Name}': {jsonEx.Message}");
					return defaultValue;
				} catch (Exception ex) {
					Debug.WriteLine($"Unexpected error deserializing JsonElement for key '{key}' to type '{typeof(T).Name}': {ex.Message}");
					return defaultValue;
				}
			}
			Debug.WriteLine($"Value for key '{key}' has unexpected type '{value.GetType().Name}' and could not be converted to '{typeof(T).Name}'.");
			return defaultValue;
		} else {
			return defaultValue;
		}
	}
}
