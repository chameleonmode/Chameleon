using Chameleon.lib.Common.Models.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;

namespace Chameleon.lib.CommunityToolkit.MvvM;

public abstract class DtoViewModelBase<T> : ViewModelObjectBase where T : Dto {
	public T Dto { get; set; }
	public DtoViewModelBase(T dto) {
		Dto = dto;
		Title = dto.title;
		Tags = dto.Tags;
	}
}

public abstract partial class ObservableDtoViewModelBase<T>(T dto, Action<ObservableDtoViewModelBase<T>>? onSelectedChanged = default)
: DtoViewModelBase<T>(dto) where T : Dto {
	public event Action<ObservableDtoViewModelBase<T>>? OnSelectedChanged = onSelectedChanged;
	[ObservableProperty] bool isSelected;
	[ObservableProperty] bool active;
	[ObservableProperty] bool isActionOptionsVisible = true;

	public override void InitializeObject() {
		base.InitializeObject();

		CommandMap["Unselect"] = () => {
			IsSelected = false;
		};

		_ = this.WhenValueChanged(x => x.IsSelected)
		.Subscribe(x => OnSelectedChanged?.Invoke(this));
	}

	partial void OnIsSelectedChanged(bool value) {
		Active = value;
		OnAnyIsSelectedChanged(value);
		OnSelectedChanged?.Invoke(this);
	}

	public virtual void OnAnyIsSelectedChanged(bool value) { }
}

