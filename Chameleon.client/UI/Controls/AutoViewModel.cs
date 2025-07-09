using Avalonia.Controls;
using Avalonia.Interactivity;
using Chameleon.client.MvvM;
using Chameleon.lib;

using System.Reflection;

namespace Chameleon.client.UI.Controls;

public class AutoViewModelInitControl : UserControl {
	protected override void OnLoaded(RoutedEventArgs e) {
		base.OnLoaded(e);
		_ = (DataContext as IInitializer)?.Initialize(e);
	}
}

public class AutoViewModelLocatorControl : AutoViewModelInitControl {
	public AutoViewModelLocatorControl() {
		DataContext ??= ViewModel ?? throw new NullReferenceException($"ViewModel for {GetType().Name} not found.");
	}

	/// <summary>
	/// locates a view model by firest looking for a view model attribute then fallse back to check the naming convention 
	/// retuns null if not fond
	/// </summary>
	/// <returns>ViewModel or Null</returns>
	protected virtual object? ViewModel {
		get {
			var viewType = GetType();
			if (viewType.GetCustomAttribute<ViewModelAttribute>()?.Type is Type t)
				return IoC.GetService(t);

			var vmt =
				Type.GetType($"{viewType.Namespace}.ViewModels.{viewType.Name}Model, {viewType.GetTypeInfo().Assembly.FullName}") ??
				Type.GetType($"{viewType.Namespace}.{viewType.Name}Model, {viewType.GetTypeInfo().Assembly.FullName}") ??
				Type.GetType($"{viewType.Namespace?.Replace(".Views", ".ViewModels")}.{viewType.Name}Model, {viewType.GetTypeInfo().Assembly.FullName}");

			ArgumentNullException.ThrowIfNull(vmt, nameof(vmt));

			return IoC.GetService(vmt);
		}
	}
}