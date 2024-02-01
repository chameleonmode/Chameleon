using Chameleon.Interfaces.AutoMapper;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Modules;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Chameleon.Infrastructure.Modules
{
    public class ModuleLoader : IModuleLoader
    {
        /// <summary>
        /// Used to search for dll are should loaded without hard reference 
        /// </summary>
        private const string ModuleFileNamePattern = "Chameleon.*.dll";
        private readonly Regex ModuleFileNameRegex = new Regex(ModuleFileNamePattern);
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly IContainerProvider _containerProvider;

        public ModuleLoader(
            IContainerProvider containerProvider
            )
        {
            _containerProvider = containerProvider;
        }

        public void LoadModules(IModuleCatalog catalog)
        {
            EnsureAllAssembliesLoaded();
            foreach (var module in GetModules())
            {
                catalog.AddModule(module);
            }
        }

        private void EnsureAllAssembliesLoaded()
        {
            RetriveAlreadyLoadedAssemblies();
            LoadAssebliesFromFileSystem();
        }

        private void LoadAssebliesFromFileSystem()
        {
            var asseblyFilePathsToLoad = Directory
                .GetFiles(AppDomain.CurrentDomain.BaseDirectory, ModuleFileNamePattern)
                .Where(filePath => !IsAssemblyLoaded(filePath))
                .ToList();

            var assebliesLoaded = asseblyFilePathsToLoad
                .Select(LoadAssembly)
                .ToList();

            // register types as soon as possible
            RegisterTypes(assebliesLoaded);
        }

        private void RegisterTypes(IList<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                _containerProvider.RegisterTypesFrom(assembly);
                _containerProvider.RegisterMapperFrom(assembly);
            }
        }

        private void RetriveAlreadyLoadedAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(assembly => {
                    try
                    {
                        // ignore exception like:
                        // The invoked member is not supported in a dynamic assembly.
                        return assembly.Location.Length > 0;
                    }
                    catch
                    {
                        return false;
                    }
                })
                .Where(IsModuleAssembly)
                .ToList();

            _assemblies.Clear();
            _assemblies.AddRange(assemblies);
        }

        private bool IsModuleAssembly(Assembly assembly)
        {
            var fileName = Path.GetFileName(assembly.Location);
            return ModuleFileNameRegex.IsMatch(fileName);
        }

        private bool IsAssemblyLoaded(string filePath)
        {
            return _assemblies.Any(a => 
                a.Location.Equals(filePath, StringComparison.OrdinalIgnoreCase)
            );
        }

        private Assembly LoadAssembly(string filePath)
        {
            var assembly = Assembly.LoadFrom(filePath);
            _assemblies.Add(assembly);
            return assembly;
        }

        private IList<ModuleInfo> GetModules()
        {
            return _assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IModule).IsAssignableFrom(type))
                .Where(type => !type.IsAbstract)
                .Select(type => new ModuleInfo(type))
                .ToList();
        }
    }
}
