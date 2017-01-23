using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WebSocketSharp;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using FChatLib.Entities;
using FChatLib.Entities.Events.Client;
using System.Collections.Specialized;
using FChatLib.Entities.EventHandlers;
using FChatLib.Entities.EventHandlers.WebSocket;
using System.Reflection;
using System.Security.Policy;
using FChatLib.Entities.Plugin;
using NuGet;

namespace FChatLib
{
    public class Bot : IBot
    {

        private string _username;
        private string _password;
        private string _botCharacterName;
        private string _administratorCharacterName;
        private bool _debug;
        private int _delayBetweenEachReconnection;
        private WebSocket wsClient;

        //plugin-name is the key, event handler is the value
        private Dictionary<string, IWebSocketEventHandler> _wsEventHandlers;

        public Dictionary<string, IWebSocketEventHandler> WSEventHandlers
        {
            get
            {
                return _wsEventHandlers;
            }

            set
            {
                _wsEventHandlers = value;
            }
        }

        public Bot(string username, string password, string botCharacterName, string administratorCharacterName)
        {
            _username = username;
            _password = password;
            _botCharacterName = botCharacterName;
            _administratorCharacterName = administratorCharacterName;
            _debug = false;
            _delayBetweenEachReconnection = 4000;
            WSEventHandlers = new Dictionary<string, IWebSocketEventHandler>();
        }

        public Bot(string username, string password, string botCharacterName, string administratorCharacterName, bool debug, int delayBetweenEachReconnection) : this(username, password, botCharacterName, administratorCharacterName)
        {
            _debug = debug;
            _delayBetweenEachReconnection = delayBetweenEachReconnection;
        }

        public Bot(string username, string password, string botCharacterName, string administratorCharacterName, bool debug, int delayBetweenEachReconnection, string pluginName, IWebSocketEventHandler eventHandler) : this(username, password, botCharacterName, administratorCharacterName, debug, delayBetweenEachReconnection)
        {
            AddPlugin(pluginName, eventHandler);
        }

        private string GetTicket()
        {
            var jsonData = JsonConvert.SerializeObject(new
            {
                account = _username,
                password = _password
            }, Formatting.Indented);

            var jsonResult = "";
            using (WebClient wc = new WebClient())
            {
                NameValueCollection vals = new NameValueCollection();
                vals.Add("account", _username);
                vals.Add("password", _password);
                var response = wc.UploadValues("https://www.f-list.net/json/getApiTicket.php", vals);
                jsonResult = Encoding.UTF8.GetString(response);
            }

            var jsonObject = JsonConvert.DeserializeObject<GetTicketResponse>(jsonResult);

            if (string.IsNullOrEmpty(jsonObject.ticket))
            {
                throw new Exception("Couldn't get authentication info from F-List API. Please restart.");
            }
            return jsonObject.ticket;
        }

        // Connection / Disconnection

        public void Connect()
        {
            //Token to authenticate on F-list
            var ticket = GetTicket();

            int port = 9722;
            if (_debug == true)
            {
                port = 8722;
            }

            wsClient = new WebSocket($"ws://chat.f-list.net:{port}");

            var identificationInfo = new Identification()
            {
                account = _username,
                botVersion = "1.0.0",
                character = _botCharacterName,
                ticket = ticket,
                method = "ticket",
                botCreator = _username
            };

            WSEventHandlers.Add("FChatLib.Default", new DefaultWebSocketEventHandler(wsClient, identificationInfo, _delayBetweenEachReconnection));

            wsClient.Connect();
        }

        public void Disconnect()
        {
            wsClient.Close(CloseStatusCode.Normal);
        }


        //Plugin related

        public void AddPlugin(string pluginName, IWebSocketEventHandler eventHandler)
        {
            RemovePlugin(pluginName);
            WSEventHandlers.Add(pluginName, eventHandler);
        }

        public void RemovePlugin(string pluginName)
        {
            if (WSEventHandlers.ContainsKey(pluginName))
            {
                WSEventHandlers.Remove(pluginName);
            }
        }

        public void InstallNuGetPlugins(string pluginName)
        {
            var repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

            var packageManager = new PackageManager(repo, System.Environment.CurrentDirectory);
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

            var packageManager = new PackageManager(repo, System.Environment.CurrentDirectory);
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

        public List<object> LoadPluginsFromAssembly(string pluginName)
        {
            List<object> loadedPlugins = new List<object>();

            try
            {
                AppDomainSetup domaininfo = new AppDomainSetup();
                domaininfo.ApplicationBase = System.Environment.CurrentDirectory;
                Evidence adevidence = AppDomain.CurrentDomain.Evidence;
                AppDomain domain = AppDomain.CreateDomain($"AD-{pluginName}", adevidence, domaininfo);

                Type type = typeof(TypeProxy);
                var value = (TypeProxy)domain.CreateInstanceAndUnwrap(
                    type.Assembly.FullName,
                    type.FullName);


                Assembly assembly = value.GetAssembly($"{System.Environment.CurrentDirectory}\\{pluginName}.dll");
                foreach (var typ in assembly.GetTypes())
                {
                    var loadedPlugin = domain.CreateInstanceAndUnwrap(typ.Assembly.FullName, typ.FullName, false, BindingFlags.Default, null, new object[] { this }, System.Globalization.CultureInfo.CurrentCulture, null);
                    if (typeof(IPlugin).IsAssignableFrom(typ) && loadedPlugin.GetType().GetMethod("OnPluginLoad") != null && loadedPlugin.GetType().GetMethod("OnPluginUnload") != null)
                    {
                        loadedPlugins.Add(loadedPlugin);
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





        // Channel related 

        public void JoinChannel(string channel)
        {
            wsClient.Send(new JoinChannel()
            {
                channel = channel
            }.ToString());
        }

        public void CreateChannel(string channelTitle)
        {
            wsClient.Send(new CreateChannel()
            {
                channel = channelTitle
            }.ToString());
        }


        // Permissions / Administration

        public bool IsUserAdmin(string character, string channel)
        {
            return (this.IsUserOP(character, channel) || this.IsUserMaster(character));
        }

        public bool IsUserOP(string character, string channel)
        {
            return true;
        }

        public bool IsUserMaster(string character)
        {
            return true;
        }

        public void KickUser(string character, string channel)
        {
            wsClient.Send(new KickFromChannel()
            {
                character = character,
                channel = channel
            }.ToString());
        }

    }
}
