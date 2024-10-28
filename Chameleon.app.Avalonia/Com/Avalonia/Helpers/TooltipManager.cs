using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;

namespace Chameleon.Avalonia.Common.Helpers;

public static class TooltipManager
{
    private static readonly Dictionary<Control, object> TooltipBackup = [];

    public static void Attach(Application app, Control control)
    {
        if (app.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
        {
            desktop.MainWindow.Deactivated += OnAppDeactivated;
            desktop.MainWindow.Activated += OnAppActivated;
        }

        void OnAppDeactivated(object? sender, System.EventArgs e)
        {
            BackupAndRemoveTooltips(control);
        }

        void OnAppActivated(object? sender, System.EventArgs e)
        {
            RestoreTooltips();
        }
    }

    private static void BackupAndRemoveTooltips(Control rootControl)
    {
        foreach (var control in FindControlsWithTooltips(rootControl))
        {
            var tooltip = ToolTip.GetTip(control);
            if (tooltip != null)
            {
                TooltipBackup[control] = tooltip;
                ToolTip.SetTip(control, null);
            }
        }
    }

    private static void RestoreTooltips()
    {
        var controlsToRestore = TooltipBackup.Keys.ToList();
        foreach (var control in controlsToRestore)
        {
            if (TooltipBackup.TryGetValue(control, out var tooltip))
            {
                ToolTip.SetTip(control, tooltip);
                TooltipBackup.Remove(control);
            }
        }
    }

    private static List<Control> FindControlsWithTooltips(Control root)
    {
        var controlsWithTooltips = new List<Control>();
        var queue = new Queue<Visual>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current is Control control && ToolTip.GetTip(control) != null)
            {
                controlsWithTooltips.Add(control);
            }

            foreach (var child in current.GetVisualChildren())
            {
                queue.Enqueue(child);
            }
        }

        return controlsWithTooltips;
    }
}