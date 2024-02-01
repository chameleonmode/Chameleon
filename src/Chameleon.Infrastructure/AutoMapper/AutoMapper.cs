using Chameleon.Interfaces.AutoMapper;
using Unity;
using System.Reflection;
using AutoMapper;
using System.Collections.Generic;
using AutoMapper.Configuration;

namespace Chameleon.Infrastructure.AutoMapper
{
    public class AutoMapper : IAutoMapper
    {
        private AutoMapperProxy _mapper = new AutoMapperProxy();
        private MapperConfigurationExpression _config = new MapperConfigurationExpression();
        private readonly List<Assembly> _assemblies = new List<Assembly>();

        public AutoMapper(
            IUnityContainer unityContainer
            )
        {
            unityContainer.RegisterInstance<IMapper>(_mapper);
        }

        public void RegisterMapper(Assembly assembly)
        {
            if (_assemblies.Contains(assembly))
            {
                return;
            }
            _assemblies.Add(assembly);
            ConfigureMappings(assembly);
            ReinitializeMapper();
        }

        private void ReinitializeMapper()
        {
            _mapper.SetConfiguration(_config);
        }

        private void ConfigureMappings(Assembly assembly)
        {
            _config.AddMaps(assembly);
        }
    }
}
