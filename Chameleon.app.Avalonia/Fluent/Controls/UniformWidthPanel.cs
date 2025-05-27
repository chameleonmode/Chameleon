using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Chameleon.app.Avalonia.Fluent.Controls;

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

public class UniformWidthStackPanel : Panel 
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<UniformWidthStackPanel, Orientation>(nameof(Orientation), Orientation.Vertical);

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize) 
    {
        var count = Children.Count;
        if (count == 0)
            return new Size(0, 0);

        if (Orientation == Orientation.Vertical)
        {
            return MeasureVertical(availableSize);
        }
        else
        {
            return MeasureHorizontal(availableSize);
        }
    }

    private Size MeasureVertical(Size availableSize)
    {
        double maxWidth = 0;
        double totalHeight = 0;

        // First pass: measure all children with unlimited width to find the widest one
        var unlimitedSize = new Size(double.PositiveInfinity, availableSize.Height);
        
        foreach (var child in Children) 
        {
            child.Measure(unlimitedSize);
            maxWidth = Math.Max(maxWidth, child.DesiredSize.Width);
        }

        // Second pass: measure all children with the uniform width
        var uniformSize = new Size(maxWidth, availableSize.Height);
        
        foreach (var child in Children) 
        {
            child.Measure(uniformSize);
            totalHeight += child.DesiredSize.Height;
        }

        return new Size(maxWidth, Math.Min(totalHeight, availableSize.Height));
    }

    private Size MeasureHorizontal(Size availableSize)
    {
        var count = Children.Count;
        double maxHeight = 0;
        double totalWidth = 0;

        // First pass: measure all children with unlimited height to find the tallest one
        var unlimitedSize = new Size(availableSize.Width, double.PositiveInfinity);
        
        foreach (var child in Children) 
        {
            child.Measure(unlimitedSize);
            maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
        }

        // Second pass: measure all children with the uniform height
        var uniformSize = new Size(availableSize.Width, maxHeight);
        
        foreach (var child in Children) 
        {
            child.Measure(uniformSize);
            totalWidth += child.DesiredSize.Width;
        }

        return new Size(Math.Min(totalWidth, availableSize.Width), maxHeight);
    }

    protected override Size ArrangeOverride(Size finalSize) 
    {
        var count = Children.Count;
        if (count == 0)
            return finalSize;

        if (Orientation == Orientation.Vertical)
        {
            return ArrangeVertical(finalSize);
        }
        else
        {
            return ArrangeHorizontal(finalSize);
        }
    }

    private Size ArrangeVertical(Size finalSize)
    {
        // Find the uniform width (widest child)
        double uniformWidth = 0;
        foreach (var child in Children) 
        {
            uniformWidth = Math.Max(uniformWidth, child.DesiredSize.Width);
        }

        double y = 0;

        foreach (var child in Children) 
        {
            // Arrange each child with uniform width, stacked vertically
            var rect = new Rect(0, y, uniformWidth, child.DesiredSize.Height);
            child.Arrange(rect);
            y += child.DesiredSize.Height;
        }

        return finalSize;
    }

    private Size ArrangeHorizontal(Size finalSize)
    {
        // Find the uniform height (tallest child)
        double uniformHeight = 0;
        foreach (var child in Children) 
        {
            uniformHeight = Math.Max(uniformHeight, child.DesiredSize.Height);
        }

        double x = 0;

        foreach (var child in Children) 
        {
            // Arrange each child with uniform height, placed horizontally
            var rect = new Rect(x, 0, child.DesiredSize.Width, uniformHeight);
            child.Arrange(rect);
            x += child.DesiredSize.Width;
        }

        return finalSize;
    }
}
