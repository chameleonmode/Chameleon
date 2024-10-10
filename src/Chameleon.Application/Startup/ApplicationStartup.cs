using Chameleon.app.Avalonia.Models;
using Chameleon.Application.Events;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.Startup;
using Chameleon.lib.Common;
using Chameleon.lib.Common.Constants;

namespace Chameleon.Application.Startup;

public class ApplicationStartup : IApplicationStartup {

	public ApplicationStartup(
		IAuthSession authSession,
		IEnumerable<IApplicationEventHandlers> _)
	{
		// for migration
		if (IoC.GetJsonValue<LoginSettings>(nameof(LoginSettings)) is null || IoC.GetJsonValue<AppSettings>(nameof(AppSettings)) is null) {
			var _settingsFilePath = Path.Combine(
					Consts.AppDataLocalDir,
					"settings.json"
					);
			if (File.Exists(_settingsFilePath)) {
				var json = File.ReadAllText(_settingsFilePath);
				var  _settings = System.Text.Json.JsonSerializer.Deserialize<Chameleon.Infrastructure.Settings.ApplicationSettings>(json);
				if (_settings is not null) {
					if(_settings.Login is not null)
					IoC.SetJsonValue(new LoginSettings(_settings.Login.LoginName, _settings.Login.LicenseKey, true),
						nameof(LoginSettings));

					if(_settings.Settings is not null)
					IoC.SetJsonValue(new AppSettings(_settings.Settings.CurrentAppTheme, _settings.Settings.CustomAccentColor?.ToString(), _settings.Settings.UseCustomAccentColor), 
						nameof(AppSettings));
				}
			}
		}
	}
}
