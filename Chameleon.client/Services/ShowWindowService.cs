using Avalonia.Controls;
using Chameleon.client.UI.Windows;
using Chameleon.lib.Services;

namespace Chameleon.client.Services;

public class ShowWindowService : IShowWindowService {
	private readonly Dictionary<object, Acrylic> windows = [];

	private void PrivateShow<TViewModel>(TViewModel viewModel, Action<TViewModel> initialize, Control view, int width, string title, Action<TViewModel>? onClosed) {
		initialize(viewModel);

		if (!windows.TryGetValue(viewModel!, out var w)) {
			w = new() {
				Topmost = true,
				WindowStartupLocation = WindowStartupLocation.CenterScreen,
				Width = width,
				Title = title,
			};
			windows[viewModel!] = w;

			w.Closed += (s, e) => {
				w.MainPanel.Children.Remove(view);
				_ = windows.Remove(viewModel!);
				onClosed?.Invoke(viewModel);
			};
			w.MainPanel.Children.Add(view);
		}

		w.Show();
	}

	public void ShowTopmost<TView, TViewModel>(Action<TViewModel> initialize,
			Action<TViewModel>? onClosed = null, string title = "CP", int width = 256) where TView : new() where TViewModel : new() {
		var vm = new TViewModel();
		ShowTopmost<TView, TViewModel>(vm, initialize, onClosed, title, width);
	}

	public void ShowTopmost<TView, TViewModel>(TViewModel vm, Action<TViewModel> initialize,
			Action<TViewModel>? onClosed = null, string title = "CP", int width = 256) where TView : new() {
		var view = new TView();
		ShowTopmost(vm, view, initialize, onClosed, title, width);
	}

	public void ShowTopmost<TView, TViewModel>(TViewModel vm, TView v, Action<TViewModel> initialize, Action<TViewModel>? onClosed, string title = "TP", int width = 256) {
		if (v is Control control) {
			control.DataContext = vm;
			PrivateShow(vm, initialize, control, width, title, onClosed);
		}
	}
}
