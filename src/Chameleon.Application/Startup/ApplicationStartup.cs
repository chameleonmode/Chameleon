using Chameleon.Application.Events;
using Chameleon.Infrastructure.Settings;
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
		if (IoC.GetJsonValue<app.lib.Models.LoginSettings>(nameof(app.lib.Models.LoginSettings)) is null) {
			var _settingsFilePath = Path.Combine(
					Consts.AppDataLocalDir,
					"settings.json"
					);
			if (File.Exists(_settingsFilePath)) {
				var json = File.ReadAllText(_settingsFilePath);
				var  _settings = System.Text.Json.JsonSerializer.Deserialize<ApplicationSettings>(json);
				if (_settings is not null) {
					IoC.SetJsonValue(new app.lib.Models.LoginSettings(_settings.Login.LoginName, _settings.Login.LicenseKey, true),
						nameof(app.lib.Models.LoginSettings));
				}
			}
		}
	}
}
