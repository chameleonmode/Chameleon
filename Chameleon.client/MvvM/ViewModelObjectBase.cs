using Chameleon.lib.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.client.MvvM;
public partial class ViewModelObjectBase : ObservableObjectBase {

	[ObservableProperty] bool showHeaderRegion = true;
	public bool Navigated { get; set; }

	public ViewModelObjectBase() {
		InitializeObject();
	}

	public ViewModelObjectBase(string? title) : this() {
		Title = title;
	}

	public virtual void InitializeObject() { }

	[RelayCommand]
	async Task Copy(object param) {
		await CopyPasta.Copy(param as string ?? "");
	}
}