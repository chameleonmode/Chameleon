using Chameleon.client.Features.AI.ChameleonAIR;
using Microsoft.Extensions.DependencyInjection;

namespace Chameleon.client.Features.AI;

public static class AIModule {
	public static IServiceCollection WithAI(this IServiceCollection services) => services
			.AddSingleton<ChameleonAIRView>()
			.AddSingleton<ChameleonAIRViewModel>();
	
}