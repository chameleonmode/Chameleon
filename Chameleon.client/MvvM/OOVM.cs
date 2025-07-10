using Chameleon.lib.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.client.MvvM;
[AttributeUsage(AttributeTargets.Class)]
public class ViewModelAttribute(Type type) : Attribute {
	public Type Type { get; private set; } = type;
}
public partial class OOVM : OO {
	[ObservableProperty] bool showHeaderRegion = true;
	public bool Navigated { get; set; }

	public OOVM() {
		InitializeObject();
	}

	public OOVM(string? title) : this() {
		Title = title;
	}

	public virtual void InitializeObject() { }

	[RelayCommand]
	public async Task Copy(object param) {
		await CopyPasta.Copy(param as string ?? "");
	}
}