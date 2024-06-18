using Chameleon.Interfaces.AutoMapper;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Chameleon.Infrastructure.Modules
{
    public abstract class ModuleLoader 
    {
        /// <summary>
        /// Used to search for dll are should loaded without hard reference 
        /// </summary>
        private const string ModuleFileNamePattern = "Chameleon.*.dll";
        private readonly Regex ModuleFileNameRegex = new Regex(ModuleFileNamePattern);
        private readonly List<Assembly> _assemblies =[];

        public ModuleLoader()
        {
        }

        public List<Assembly> Assemblies => _assemblies;

        public void LoadModules()
        {
            EnsureAllAssembliesLoaded();          
        }

        private void EnsureAllAssembliesLoaded()
        {
            RetriveAlreadyLoadedAssemblies();
        }

        public abstract void RegisterTypes(IList<Assembly> assemblies);

        private void RetriveAlreadyLoadedAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(assembly => {
                    try
                    {
                        // ignore exception like:
                        // The invoked member is not supported in a dynamic assembly.
                        return AppContext.BaseDirectory.Length > 0;
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
            var fileName = Path.GetFileName(assembly.GetName().Name);
            return ModuleFileNameRegex.IsMatch(fileName);
        }
    }
}
