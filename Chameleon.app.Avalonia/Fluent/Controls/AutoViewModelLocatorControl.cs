using Avalonia.Controls;
using Avalonia.Interactivity;

using Chameleon.lib;
using Chameleon.lib.Common.Interfaces.Sys;

using System.Reflection;

namespace Chameleon.Av.Fluent.Common.Controls;
public class AutoViewModelInitControl : UserControl {
	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);
		if (DataContext is IAmInitializer i) {
			_ = i.InitializeAsync(e);
		}
	}
}

public class AutoViewModelLocatorControl : AutoViewModelInitControl {
	public AutoViewModelLocatorControl()
	{
		DataContext ??= AutoLocateVM() ??
				throw new NullReferenceException($"ViewModel for {GetType().Name} not found.");
	}

	/// <summary>
	/// locates a view model by firest looking for a view model attribute then fallse back to check the naming convention 
	/// retuns null if not fond
	/// </summary>
	/// <returns>ViewModel or Null</returns>
	private object? AutoLocateVM()
	{
		var viewType = GetType();
		if(viewType.GetCustomAttribute<Chameleon.lib.Common.Attributes.ViewModelAttribute>()?.Type is Type t)
			return IoC.GetService(t);

		var vmt =
			Type.GetType($"{viewType.Namespace}.ViewModels.{viewType.Name}Model, {viewType.GetTypeInfo().Assembly.FullName}") ??
			Type.GetType($"{viewType.Namespace?.Replace(".Views", ".ViewModels")}.{viewType.Name}Model, {viewType.GetTypeInfo().Assembly.FullName}");

		ArgumentNullException.ThrowIfNull(vmt, nameof(vmt));

		return IoC.GetService(vmt);
	}
}