using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace Chameleon.client.UI.Controls;

// TODO
public class ItemzControl : ItemsControl {
  public static readonly StyledProperty<Control?> TemplaterProperty =
      AvaloniaProperty.Register<ItemzControl, Control?>(nameof(Templater));

  public Control? Templater {
    get => GetValue(TemplaterProperty);
    set => SetValue(TemplaterProperty, value);
  }

  static ItemzControl() {
    _ = TemplaterProperty.Changed.AddClassHandler<ItemzControl>((x, e) => x.ApplyTemplateOverride());
  }

  private void ApplyTemplateOverride() {
    if (Templater != null) {
      // Apply inline template for each item
      ItemTemplate = new FuncDataTemplate<object>((_, _) =>
          CreateControlInstance(Templater) ?? new TextBlock { Text = "⚠ Failed to create template" },
          supportsRecycling: false);
    }
  }

  private Control? CreateControlInstance(Control template) {
    try {
      var type = template.GetType();
      return (Control?)Activator.CreateInstance(type);
    } catch {
      return null;
    }
  }
}