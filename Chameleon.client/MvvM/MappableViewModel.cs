using System.Reflection;
using DynamicData.Binding;
using CommunityToolkit.Mvvm.ComponentModel;
using Chameleon.lib.Common.Models.Dto;

namespace Chameleon.client.MvvM;

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

public abstract class MappableViewModelBase<T>(T dto) : DtoViewModelBase<T>(dto) where T : Dto {
  public virtual T ToDto() {
    var viewModelType = GetType();
    var dtoType = typeof(T);

    var viewModelProperties = viewModelType
    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    .Where(p => p.CanRead);

    var dtoProperties = dtoType
    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    .Where(p => p.CanWrite)
    .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

    foreach (var vmProp in viewModelProperties) {
      if (dtoProperties.TryGetValue(vmProp.Name, out var dtoProp) &&
        dtoProp.PropertyType.IsAssignableFrom(vmProp.PropertyType)) {
        var value = vmProp.GetValue(this);
        dtoProp.SetValue(Dto, value);
      }
    }

    return Dto;
  }
}
