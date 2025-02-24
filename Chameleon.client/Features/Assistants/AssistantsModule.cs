using Chameleon.app.Features.Assistants.UserTaskforce;
using Microsoft.Extensions.DependencyInjection;

namespace Chameleon.app.Features.Assistants;

public static class AssistantsModule {
	public static IServiceCollection WithAssistants(this IServiceCollection services) => services
			.AddSingleton<UserTaskforceView>()
			.AddSingleton<UserTaskforceViewModel>();
}