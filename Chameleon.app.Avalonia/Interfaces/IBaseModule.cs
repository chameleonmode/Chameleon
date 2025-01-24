using Microsoft.Extensions.DependencyInjection;

namespace Chameleon.app.Avalonia.Interfaces;
public interface IBaseModule {
	public void ConfigureServices(IServiceCollection services);
}
