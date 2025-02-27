using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.lib.CommunityToolkit.MvvM;
public abstract class ViewModelObjectDto<T> : ViewModelObjectBase
	where T : Common.Models.Interfaces.Dto {
	public T? Dto { get; set; }

	public ViewModelObjectDto(string? title) : base(title)
	{
	}
	public ViewModelObjectDto(T dto) : base()
	{
		Dto = dto;
	}
	public ViewModelObjectDto() : base()
	{
	}
}

public abstract partial class ObservableViewModelDto<T> : ViewModelObjectDto<T>
	where T : Common.Models.Interfaces.Dto {

	[ObservableProperty]
	private bool isSelected;

	[ObservableProperty]
	private bool isActionOptionsVisible = true;

	public ObservableViewModelDto(string? title) : base(title)
	{
		CommandMap["Unselect"] = () => {
			IsSelected = false;
		};
	}

	public ObservableViewModelDto(T dto) : base(dto)
	{
		CommandMap["Unselect"] = () => {
			IsSelected = false;
		};
	}

	partial void OnIsSelectedChanged(bool value)
	{
		OnAnyIsSelectedChanged(value);
	}

	public virtual void OnAnyIsSelectedChanged(bool value){

	}
}
