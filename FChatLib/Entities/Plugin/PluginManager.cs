using FChatLib.Entities.EventHandlers;
using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib.Entities.Plugin
{
    public class PluginManager
    {

        public List<PluginSpawner> PluginSpawnersList;

        //             key=channel     values=pluginName,pluginClass
        public Dictionary<string, Dictionary<string, BasePlugin>> LoadedPlugins;

        public Bot Bot;

        public PluginManager(Bot myBot)
        {
            Bot = myBot;
            LoadedPlugins = new Dictionary<string, Dictionary<string, BasePlugin>>();
            LoadAllAvailablePlugins();
            Bot.Events.ReceivedPluginCommand += PassCommandToLoadedPlugins;
        }

        public void PassCommandToLoadedPlugins(object sender, ReceivedPluginCommandEventArgs e)
        {
            if (LoadedPlugins.ContainsKey(e.Channel.ToLower()))
            {
                foreach(var plugin in LoadedPlugins[e.Channel.ToLower()].Values)
                {
                    plugin.ExecuteCommand(e.Command, e.Arguments);
                }
            }
        }

        public void LoadAllAvailablePlugins()
        {
            PluginSpawnersList = new List<PluginSpawner>();
            var availablePlugins = GetAvailablePlugins();

            foreach (var pluginName in availablePlugins)
            {
                LoadPluginsFromAssembly(pluginName.ToLower());
            }
        }

        public void CreatePluginManagerForChannelIfNotExistent(string channel)
        {
            if (!LoadedPlugins.ContainsKey(channel.ToLower()))
            {
                LoadedPlugins.Add(channel.ToLower(), new Dictionary<string, BasePlugin>());
            }
        }

        public bool LoadPlugin(Bot myBot, string channel, string pluginName)
        {
            var myPlugin = GetPluginInstance(pluginName.ToLower(), myBot);
            var flag = false;

            if(myPlugin != null)
            {
                CreatePluginManagerForChannelIfNotExistent(channel.ToLower());

                if (LoadedPlugins[channel.ToLower()].ContainsKey(pluginName.ToLower()))
                {
                    LoadedPlugins[channel.ToLower()][pluginName.ToLower()] = myPlugin;
                    flag = true;
                }
                else
                {
                    LoadedPlugins[channel.ToLower()].Add(pluginName.ToLower(), myPlugin);
                    flag = true;
                }
            }

            return flag;
        }

        public bool UpdatePlugin(string pluginName)
        {
            bool success = true;
            try
            {
                PluginSpawnersList.RemoveAll(x => x.PluginName.ToLower() == pluginName.ToLower());
                LoadPluginsFromAssembly(pluginName);
            }
            catch (Exception)
            {
                success = false;
            }
            
            return success;
        }

        public bool UpdateAllPlugins()
        {
            bool success = true;
            try
            {
                PluginSpawnersList.Clear();
                LoadAllAvailablePlugins();
            }
            catch (Exception)
            {
                success = false;
            }

            return success;
        }

        public bool ReloadPluginGlobal(Bot myBot, string pluginName)
        {
            var myPlugin = GetPluginInstance(pluginName.ToLower(), myBot);
            var flag = false;

            if (myPlugin != null)
            {
                foreach (var channel in LoadedPlugins.Keys)
                {
                    if (LoadedPlugins[channel].ContainsKey(pluginName.ToLower()))
                    {
                        LoadedPlugins[channel][pluginName.ToLower()] = myPlugin;
                        flag = true;
                    }
                    else
                    {
                        LoadedPlugins[channel].Add(pluginName.ToLower(), myPlugin);
                        flag = true;
                    }
                }
            }

            return flag;
        }

        public bool ReloadPluginInChannel(Bot myBot, string channel, string pluginName)
        {
            var myPlugin = GetPluginInstance(pluginName.ToLower(), myBot);

            if (myPlugin != null)
            {
                CreatePluginManagerForChannelIfNotExistent(channel.ToLower());
                LoadedPlugins[channel.ToLower()].Add(pluginName.ToLower(), myPlugin);
                return true;
            }

            return false;
        }

        public BasePlugin GetPluginInstance(string pluginName, Bot myBot)
        {
            foreach (var pluginSpawner in PluginSpawnersList)
            {
                if(pluginSpawner.PluginName.ToLower() == pluginName.ToLower())
                {
                    var loadedPlugin = (BasePlugin)pluginSpawner.Domain.CreateInstanceAndUnwrap(pluginSpawner.AssemblyName, pluginSpawner.TypeName, false, BindingFlags.Default, null, new object[] { myBot }, System.Globalization.CultureInfo.CurrentCulture, null);
                    return loadedPlugin;
                }
            }

            return null;
        }

        public List<string> GetAvailablePlugins()
        {
            var unformattedPluginsList = System.IO.Directory.EnumerateFiles(Environment.CurrentDirectory, "FChatLib.Plugin.*.dll");
            var formattedPluginsList = unformattedPluginsList.ToList();
            formattedPluginsList.Select(x => x.Replace("FChatLib.Plugin.", "").Replace(".dll", "").ToLower());
            return formattedPluginsList;
        }

        public void InstallNuGetPlugins(string pluginName)
        {
            var repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

            var packageManager = new PackageManager(repo, Environment.CurrentDirectory);
            //packageManager.PackageInstalled += PackageManager_PackageInstalled;


            var package = repo.FindPackage(pluginName);
            if (package != null)
            {
                packageManager.InstallPackage(package, false, true);
            }
        }

        public void UpdateNuGetPlugins(IEnumerable<NuGet.IPackageName> pluginNames)
        {
            var repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

            var packageManager = new PackageManager(repo, Environment.CurrentDirectory);
            //packageManager.PackageInstalled += PackageManager_PackageInstalled;


            var packagesToUpdate = repo.GetUpdates(pluginNames, true, true);
            if (packagesToUpdate.Any())
            {
                foreach (var package in packagesToUpdate)
                {
                    Console.WriteLine($"Updating package {package.GetFullName()} to version {package.Version}");
                    packageManager.UpdatePackage(package, true, true);
                }
            }
        }

        public List<PluginSpawner> LoadPluginsFromAssembly(string pluginName)
        {
            List<PluginSpawner> loadedPlugins = new List<PluginSpawner>();
            var wrapper = new PluginSpawner();
            try
            {
                AppDomainSetup domaininfo = new AppDomainSetup()
                {
                    ApplicationBase = Environment.CurrentDirectory,
                    ShadowCopyDirectories = Environment.CurrentDirectory,
                    ShadowCopyFiles = "true"
                };
                Evidence adevidence = AppDomain.CurrentDomain.Evidence;
                AppDomain domain = AppDomain.CreateDomain($"AD-{pluginName}", adevidence, domaininfo);

                Type type = typeof(TypeProxy);
                var value = (TypeProxy)domain.CreateInstanceAndUnwrap(
                    type.Assembly.FullName,
                    type.FullName);


                Assembly assembly = value.GetAssembly($"{System.Environment.CurrentDirectory}\\FChatLib.Plugin.{pluginName}.dll");

                foreach (var typ in assembly.GetTypes())
                {
                    
                    if (typeof(IPlugin).IsAssignableFrom(typ))
                    {
                        var loadedPlugin = domain.CreateInstanceAndUnwrap(typ.Assembly.FullName, typ.FullName, false, BindingFlags.Default, null, new object[] { this }, System.Globalization.CultureInfo.CurrentCulture, null);
                        if(loadedPlugin.GetType().GetMethod("OnPluginLoad") != null && loadedPlugin.GetType().GetMethod("OnPluginUnload") != null)
                        {
                            var pluginWrapper = new PluginSpawner()
                            {
                                Assembly = assembly,
                                Domain = domain,
                                AssemblyName = typ.Assembly.FullName,
                                TypeName = typ.FullName,
                                PluginFileName = $"FChatLib.Plugin.{ pluginName }.dll",
                                PluginName = pluginName.ToLower(),
                                PluginVersion = assembly.GetName().Version
                            };
                            loadedPlugins.Add(pluginWrapper);
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed to load plugin {0}", pluginName);
                Console.WriteLine(ex.ToString());
            }

            return loadedPlugins;

        }
    }
}
