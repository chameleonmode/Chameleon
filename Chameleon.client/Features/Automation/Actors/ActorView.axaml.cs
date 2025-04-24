using Avalonia;
using Chameleon.Av.Fluent.Common.Pages;
using FluentAvalonia.UI.Controls;

namespace Chameleon.client.Features.Automation.Actors;

public partial class ActorView : ChameleonPageBase {
  public ActorView() {
    InitializeComponent();
    ControlName = "Mr. Actor";
    Description = "AI Robot Agent & Automationed Actor";
    PreviewImage = App.TryGetResource<IconSource>("SpiderIcon");
  }
  
  // public override Visual? AnimateVisual { get => UPView; set => base.AnimateVisual = value; }
}