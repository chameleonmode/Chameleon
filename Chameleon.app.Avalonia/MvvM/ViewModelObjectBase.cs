using Chameleon.lib.Helpers;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.lib.CommunityToolkit.MvvM;
public partial class ViewModelObjectBase : ObservableObjectBase {
	public bool Navigated { get; set; }

	public ViewModelObjectBase() {
		InitCommandMapping();
	}

	public ViewModelObjectBase(string? title) : this() {
		Title = title;
	}

	public virtual void InitCommandMapping() { }

	[RelayCommand]
	async Task Copy(object param) {
		await CopyPasta.Copy(param as string ?? "");
	}
}