using Chameleon.lib.Common.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.lib.CommunityToolkit.MvvM;
public abstract class DtoViewModelBase<T>(T dto, string? title = null)
 : ViewModelObjectBase(title ?? dto.title)	where T : Dto {
	public T Dto { get; set; } = dto;
}

public abstract partial class ObservableDtoViewModelBase<T>(
	T dto, 
	string? title = null, 
 Action<ObservableDtoViewModelBase<T>>? onSelectedChanged = default
) : DtoViewModelBase<T>(dto, title) where T : Dto {
	[ObservableProperty]
	private bool isSelected;
	[ObservableProperty]
	private bool isActionOptionsVisible = true;

	public override void InitCommandMapping() {
		CommandMap["Unselect"] = () => {
			IsSelected = false;
		};
	}

	partial void OnIsSelectedChanged(bool value) {
		OnAnyIsSelectedChanged(value);
		onSelectedChanged?.Invoke(this);
	}

	public virtual void OnAnyIsSelectedChanged(bool value) { }
}
