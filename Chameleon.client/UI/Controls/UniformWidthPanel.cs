using Avalonia;
using Avalonia.Controls;

namespace Chameleon.client.UI.Controls;

public class UniformWidthPanel : Panel {
  protected override Size MeasureOverride(Size availableSize) {
    var count = Children.Count;
    if (count == 0)
      return new Size(0, 0);

    // Divide the available width equally among children.
    var childWidth = availableSize.Width / count;
    var childAvailableSize = new Size(childWidth, availableSize.Height);
    double maxHeight = 0;

    foreach (var child in Children) {
      child.Measure(childAvailableSize);
      maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
    }

    return new Size(availableSize.Width, maxHeight);
  }

  protected override Size ArrangeOverride(Size finalSize) {
    var count = Children.Count;
    if (count == 0)
      return finalSize;

    var childWidth = finalSize.Width / count;
    double x = 0;

    foreach (var child in Children) {
      // Arrange each child with the equal width and let its height be its desired height.
      var rect = new Rect(x, 0, childWidth, child.DesiredSize.Height);
      child.Arrange(rect);
      x += childWidth;
    }

    return finalSize;
  }
}

