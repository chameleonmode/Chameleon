using CommunityToolkit.Mvvm.ComponentModel;

namespace Chameleon.client.Features.Dashboard.Tags;

public partial class TagViewModel(Action<TagViewModel> OnSelectChanged) : ObservableObject {
  [ObservableProperty] string name = null!;
  [ObservableProperty] bool isSelected;

  partial void OnIsSelectedChanged(bool value) {
    if (value) OnSelectChanged(this);
  }
}