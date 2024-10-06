using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Auth;
using Chameleon.lib.Common.ServiceManagers;

namespace Chameleon.app.Avalonia;
public class AppStartup {
	public event Action? OnLoginSuccess;

	private readonly IAuthService? _authService;

	public async Task RunAsync()
	{
		if (!await RunAsync(0)) {
			_ = await Mbox.ShowErrorAsync("Error Logging In", "There was an error validationg the login information that was provided.");
			Environment.Exit(0);
		} else {
			OnLoginSuccess?.Invoke();
		}
	}
	public async Task<bool> RunAsync(int trys)
	{
		if(_authService == null)
			return false;

		bool success;
		try {
			success = await _authService.LoginAsync();
			if (!success)
				success = await _authService.ShowLoginDialogAsync();
		} catch {
			if (trys < 1)
				return await RunAsync(trys);

			success = false;
		}
		return success;
	}

	public static AppStartup Instance { get; } = new AppStartup();
	private AppStartup()
	{
		_authService = ContainerServiceHelper.Resolve<IAuthService>();
	}
}
