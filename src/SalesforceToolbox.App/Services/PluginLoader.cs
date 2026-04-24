using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SalesforceToolbox.Core.Interfaces;

namespace SalesforceToolbox.App.Services
{
    public class PluginLoader
    {
        private static readonly string PluginsFolder = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppDomain.CurrentDomain.BaseDirectory,
            "Plugins");

        public IEnumerable<IPlugin> LoadPlugins(ISalesforceService salesforceService)
        {
            var plugins = new List<IPlugin>();

            if (!Directory.Exists(PluginsFolder))
                return plugins;

            foreach (var dllPath in Directory.GetFiles(PluginsFolder, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dllPath);
                    var pluginTypes = assembly.GetExportedTypes()
                        .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass);

                    foreach (var type in pluginTypes)
                    {
                        try
                        {
                            var plugin = (IPlugin)Activator.CreateInstance(type);
                            plugin.Initialize(salesforceService);
                            plugins.Add(plugin);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to load plugin {type.FullName}: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load assembly {dllPath}: {ex.Message}");
                }
            }

            return plugins;
        }
    }
}
