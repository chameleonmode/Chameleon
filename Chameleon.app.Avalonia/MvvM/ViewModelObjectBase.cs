using Chameleon.lib.Auth;
using Chameleon.lib.Helpers;

using CommunityToolkit.Mvvm.Input;

namespace Chameleon.lib.CommunityToolkit.MvvM;
public partial class ViewModelObjectBase : ObservableObjectBase {
	public Session CurrentSession { get; } = Session.Instance;

	public bool Navigated { get; set; }
	public ViewModelObjectBase() {

	}

	public ViewModelObjectBase(string? title) : this() {
		Title = title;
	}

	public ViewModelObjectBase(string title, Func<ViewModelObjectBase> init) : this(title) {
		_ = init();
	}

	[RelayCommand]
	async Task Copy(object param) {
		await CopyPasta.Copy(param as string ?? "");
	}
}