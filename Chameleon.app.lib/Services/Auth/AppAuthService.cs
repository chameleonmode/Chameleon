
using Chameleon.app.lib.Models;
using Chameleon.lib.Api;
using Chameleon.lib.Common;
using Chameleon.lib.Common.Extensions;

namespace Chameleon.app.lib.Services.Auth;
public class AppAuthService {
	public async Task<bool> LoginAsync(Action<LoginResponse> @onSuccess)
	{
		LoginResponse? loginResult = null;
		try {
			var loginSettings = IoC.GetJsonValue<LoginSettings>(nameof(LoginSettings));
			if (loginSettings is null || loginSettings.AutoLogin == false)
				return false;

			if (loginSettings.LoginName.Is() && loginSettings.LicenseKey.Is()) {
				loginResult = await Auther.LoginAsync(loginSettings.LoginName, loginSettings.LicenseKey);
			}
		} catch {
			loginResult = null;
		} finally {
			if (loginResult is not null) {
				@onSuccess(loginResult);
			}
		}

		return loginResult is not null && loginResult.AccessToken.Is();
	}

	public static AppAuthService Instance { get; } = new AppAuthService();
	private AppAuthService() { }
}
