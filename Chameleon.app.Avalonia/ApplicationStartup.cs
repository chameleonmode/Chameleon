using Chameleon.Interfaces.Auth;
using Chameleon.lib.Common.ServiceManagers;

namespace Chameleon.app.Avalonia;
//internal class ApplicationStartup {
//	public async Task RunAsync()
//	{
//		if (!await RunAsync(0)) {
//			_ = await Mbox.ShowErrorAsync("Error Logging In", "There was an error validationg the login information that was provided.");
//			CloseApplication();
//		} else
//			_eventAggregator
//						 .GetEvent<LoginSuccessEvent>()
//						 .Publish(new LoginEventArgs(null));
//	}
//	public async Task<bool> RunAsync(int trys)
//	{
//		bool success;
//		try {
//			success = await _authService.LoginAsync();
//			if (!success)
//				success = await _authService.ShowLoginDialogAsync();
//		} catch {
//			if (trys < 1)
//				return await RunAsync(trys);

//			success = false;
//		}
//		return success;
//	}

//	private void CloseApplication()
//	{
//		Environment.Exit(0);
//	}

//	public void Run()
//	{
//		_authService.Login();
//	}
//}
