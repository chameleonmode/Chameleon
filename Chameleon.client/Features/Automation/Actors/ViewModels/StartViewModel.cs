using Chameleon.AIR.Actors.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using ExCSS;
using HarfBuzzSharp;

namespace Chameleon.client.Features.Automation.Actors.ViewModels;
public partial class StartViewModel : ObservableObject {
	[ObservableProperty] string _feature = "";
	[ObservableProperty] string? _url;
	[ObservableProperty] bool? _new;

	public StartViewModel() { }
	public StartViewModel(Start source) {
		Feature = source.Feature;
		Url = source.Url;
		New = source.New;
	}
	public Start ToRecord() => new(Feature, Url, New);
}
