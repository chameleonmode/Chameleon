using System.Reactive.Subjects;
using DynamicData.Binding;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Reactive.Linq;

namespace Chameleon.client.Features.Shared.Tags;

public partial class TagViewModel : ObservableObject {
  [ObservableProperty] string name = null!;
  [ObservableProperty] bool isSelected;

  public BehaviorSubject<TagViewModel> TagObservable { get; }

  public TagViewModel() {
    TagObservable = new(this);

    _ = this.WhenValueChanged(x => x.IsSelected)
                .Where(isSelected => isSelected)
                .Subscribe(_ => TagObservable.OnNext(this));
  }
}