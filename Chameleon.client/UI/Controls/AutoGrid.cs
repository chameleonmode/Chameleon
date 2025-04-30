
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Styling;

namespace Chameleon.client.UI.Controls;

/// <summary>
/// TODO: finish
/// </summary>
public class AutoGrid : Panel {
    private Grid _internalGrid;

    // Property to enable/disable automatic class application
    public static readonly StyledProperty<bool> AutoApplyClassesProperty =
        AvaloniaProperty.Register<AutoGrid, bool>(nameof(AutoApplyClasses), defaultValue: false);

    public bool AutoApplyClasses {
        get => GetValue(AutoApplyClassesProperty);
        set => SetValue(AutoApplyClassesProperty, value);
    }

    // Style class to apply to column 0 elements (typically labels)
    public static readonly StyledProperty<string> LabelClassProperty =
        AvaloniaProperty.Register<AutoGrid, string>(nameof(LabelClass), defaultValue: "formElement");

    public string LabelClass {
        get => GetValue(LabelClassProperty);
        set => SetValue(LabelClassProperty, value);
    }

    // Style class to apply to column 1 elements (typically inputs)
    public static readonly StyledProperty<string> InputClassProperty =
        AvaloniaProperty.Register<AutoGrid, string>(nameof(InputClass), defaultValue: "formInput");

    public string InputClass {
        get => GetValue(InputClassProperty);
        set => SetValue(InputClassProperty, value);
    }

    // Attached property to specify the column
    public static readonly AttachedProperty<int> ColumnProperty =
        AvaloniaProperty.RegisterAttached<AutoGrid, Control, int>("Column", defaultValue: 0);

    public static void SetColumn(Control control, int value) {
        control.SetValue(ColumnProperty, value);
    }

    public static int GetColumn(Control control) {
        return control.GetValue(ColumnProperty);
    }

    public AutoGrid() {
        // Setup styles in constructor
        this.Styles.Add(new Avalonia.Styling.Style(x => x.OfType<Control>().Class("formElement")) {
            Setters = { new Setter(Control.MarginProperty, new Thickness(5)) }
        });

        this.Styles.Add(new Avalonia.Styling.Style(x => x.OfType<TextBlock>().Class("formElement")) {
            Setters = { new Setter(TextBlock.VerticalAlignmentProperty, Avalonia.Layout.VerticalAlignment.Center) }
        });

        this.Styles.Add(new Avalonia.Styling.Style(x => x.OfType<Control>().Class("formInput")) {
            Setters = {
                    new Setter(Control.MarginProperty, new Thickness(5)),
                    new Setter(Control.HeightProperty, 32.0),
                    new Setter(Control.HorizontalAlignmentProperty, Avalonia.Layout.HorizontalAlignment.Stretch)
                }
        });

        this.Styles.Add(new Avalonia.Styling.Style(x => x.OfType<TextBox>().Class("formInput")) {
            Setters = { new Setter(TextBox.VerticalContentAlignmentProperty, Avalonia.Layout.VerticalAlignment.Center) }
        });

        this.Styles.Add(new Avalonia.Styling.Style(x => x.OfType<ComboBox>().Class("formInput")) {
            Setters = { new Setter(ComboBox.VerticalContentAlignmentProperty, Avalonia.Layout.VerticalAlignment.Center) }
        });

        this.Styles.Add(new Avalonia.Styling.Style(x => x.OfType<NumericUpDown>().Class("formInput")) {
            Setters = { new Setter(NumericUpDown.VerticalContentAlignmentProperty, Avalonia.Layout.VerticalAlignment.Center) }
        });

        _internalGrid = new Grid();
        _internalGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto, MinWidth = 100 });
        _internalGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        // Add the grid to our visual children
        Children.Add(_internalGrid);

        AttachedToLogicalTree += AutoGrid_AttachedToLogicalTree;
        Children.CollectionChanged += Children_CollectionChanged;
    }

    private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        // Handle new items being added
        if (e.NewItems != null) {
            foreach (var item in e.NewItems) {
                if (item is Control control && control != _internalGrid) {
                    ProcessChild(control);
                }
            }
        }
    }

    private void AutoGrid_AttachedToLogicalTree(object? sender, LogicalTreeAttachmentEventArgs e) {
        // Process existing children
        foreach (var child in Children) {
            if (child is Control control && control != _internalGrid) {
                ProcessChild(control);
            }
        }
    }

    private int _currentRow = 0;
    private int _columnInCurrentRow = 0; // Track which column we're currently filling in the current row

    private void ProcessChild(Control control) {
        // Skip if the control is the internal grid
        if (control == _internalGrid)
            return;

        // Skip if already processed
        if (_internalGrid.Children.Contains(control))
            return;

        // If the control is a Panel, process its children instead
        if (control is Panel panel) {
            foreach (var child in panel.Children.ToList()) {
                if (child is Control childControl) {
                    ProcessChild(childControl);
                }
            }

            // Now remove the panel from our children since we've processed its children
            if (Children.Contains(panel)) {
                Children.Remove(panel);
            }
            return;
        }
        var clm = control.GetValue(ColumnProperty);
        // Get the column from the attached property or use the current column in row
        int column;
        if (control.IsSet(ColumnProperty)) {
            // Use explicit column if specified
            column = GetColumn(control);

            // If column is explicitly set to 0 and we're already in column 1 of this row,
            // move to the next row
            if (column == 0 && _columnInCurrentRow == 1) {
                _currentRow++;
                _columnInCurrentRow = 0;
            }
            // If column is explicitly set to 1 and we're already in column 1 of this row,
            // move to the next row
            else if (column == 1 && _columnInCurrentRow == 1) {
                _currentRow++;
                _columnInCurrentRow = 1;
            } else {
                _columnInCurrentRow = column;
            }
        } else {
            // Use the next column in the current row
            column = _columnInCurrentRow;

            // Move to next column or row
            _columnInCurrentRow++;
            if (_columnInCurrentRow > 1) {
                _columnInCurrentRow = 0;
                _currentRow++;
            }
        }

        // Apply appropriate form class if enabled
        if (AutoApplyClasses) {
            ApplyFormClasses(control, column);
        }

        // Move the control from our Children to the internal grid
        if (Children.Contains(control)) {
            Children.Remove(control);
        }

        // Add a new row definition if needed
        if (_currentRow >= _internalGrid.RowDefinitions.Count) {
            _internalGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        // Set the grid properties on the control
        Grid.SetRow(control, _currentRow);
        Grid.SetColumn(control, column);

        // Add the control to the internal grid
        _internalGrid.Children.Add(control);
    }

    private void ApplyFormClasses(Control control, int column) {
        if (control.Classes == null)
            return;

        string classToApply = column == 0 ? LabelClass : InputClass;

        if (!string.IsNullOrEmpty(classToApply) && !control.Classes.Contains(classToApply)) {
            control.Classes.Add(classToApply);
        }
    }

    protected override Size MeasureOverride(Size availableSize) {
        // Just measure our internal grid and return its desired size
        _internalGrid.Measure(availableSize);
        return _internalGrid.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize) {
        // Arrange our internal grid to fill the available space
        _internalGrid.Arrange(new Rect(finalSize));
        return finalSize;
    }
}
