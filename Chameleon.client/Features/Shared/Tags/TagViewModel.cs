using DynamicData.Binding;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Reactive.Linq;

namespace Chameleon.client.Features.Shared.Tags;

public partial class TagViewModel(Action<TagViewModel> OnSelectChanged) : ObservableObject {
  [ObservableProperty] string name = null!;
  [ObservableProperty] bool isSelected;

  partial void OnIsSelectedChanged(bool value) {
    if (value) OnSelectChanged(this);
  }
}