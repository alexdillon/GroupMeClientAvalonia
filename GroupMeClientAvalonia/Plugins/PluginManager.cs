using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using GroupMeClientPlugin;
using GroupMeClientPlugin.GroupChat;
using GroupMeClientPlugin.MessageCompose;

namespace GroupMeClientAvalonia.Plugins
{
    /// <summary>
    /// <see cref="PluginManager"/> provides functionality to dynamically load <see cref="IPluginBase"/>-based
    /// plugins and register them to extend Client functionality.
    /// </summary>
    /// <remarks>
    /// Based on https://code.msdn.microsoft.com/windowsdesktop/Creating-a-simple-plugin-b6174b62.
    /// Adapted for .Net Core based on https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support
    /// </remarks>
    public sealed class PluginManager
    {
        private static readonly Lazy<PluginManager> LazyPluginManager = new Lazy<PluginManager>(() => new PluginManager());

        private PluginManager()
        {
        }

        /// <summary>
        /// Gets the instance of the <see cref="PluginManager"/> for the current application.
        /// </summary>
        public static PluginManager Instance => LazyPluginManager.Value;

        /// <summary>
        /// Gets the available <see cref="IGroupChatPlugin"/> plugins.
        /// </summary>
        public ICollection<IGroupChatPlugin> GroupChatPlugins { get; } = new List<IGroupChatPlugin>();

        /// <summary>
        /// Gets the available <see cref="IGroupChatCachePlugin"/> plugins.
        /// </summary>
        public ICollection<IGroupChatCachePlugin> GroupChatCachePlugins { get; } = new List<IGroupChatCachePlugin>();

        /// <summary>
        /// Gets the available <see cref="IMessageComposePlugin"/> plugins.
        /// </summary>
        public ICollection<IMessageComposePlugin> MessageComposePlugins { get; } = new List<IMessageComposePlugin>();

        /// <summary>
        /// Loads and registers all available plugins.
        /// </summary>
        /// <param name="pluginsPath">The folder to load plugins from.</param>
        public void LoadPlugins(string pluginsPath)
        {
            if (!Directory.Exists(pluginsPath))
            {
                return;
            }

            string[] dllFileNames = Directory.GetFiles(pluginsPath, "*.dll");

            ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
            foreach (string dllFile in dllFileNames)
            {
                Assembly pluginAssembly = LoadPlugin(dllFile);
                assemblies.Add(pluginAssembly);
            }

            Type pluginType = typeof(PluginBase);
            ICollection<Type> pluginTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    if (assembly != null)
                    {
                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            if (type.IsInterface || type.IsAbstract)
                            {
                                continue;
                            }
                            else
                            {
                                if (type.IsSubclassOf(pluginType))
                                {
                                    pluginTypes.Add(type);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error loading plugin {0}. Error Code: {1}", assembly.FullName, ex.Message);
                }
            }

            this.GroupChatPlugins.Clear();
            this.MessageComposePlugins.Clear();

            foreach (Type type in pluginTypes)
            {
                var plugin = (PluginBase)Activator.CreateInstance(type);

                if (plugin is IMessageComposePlugin messageComposePlugin)
                {
                    this.MessageComposePlugins.Add(messageComposePlugin);
                }
                else if (plugin is IGroupChatPlugin groupChatPlugin)
                {
                    this.GroupChatPlugins.Add(groupChatPlugin);
                }
                else if (plugin is IGroupChatCachePlugin groupChatCachePlugin)
                {
                    this.GroupChatCachePlugins.Add(groupChatCachePlugin);
                }
            }
        }

        private static Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading commands from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        private class PluginLoadContext : AssemblyLoadContext
        {
            private readonly AssemblyDependencyResolver resolver;

            public PluginLoadContext(string pluginPath)
            {
                this.resolver = new AssemblyDependencyResolver(pluginPath);
            }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                string assemblyPath = this.resolver.ResolveAssemblyToPath(assemblyName);
                if (assemblyPath != null)
                {
                    return this.LoadFromAssemblyPath(assemblyPath);
                }

                return null;
            }

            protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
            {
                string libraryPath = this.resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
                if (libraryPath != null)
                {
                    return this.LoadUnmanagedDllFromPath(libraryPath);
                }

                return IntPtr.Zero;
            }
        }
    }
}
