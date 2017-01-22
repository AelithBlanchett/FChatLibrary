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
using FChatLib.Entities.Commands;
using System.Collections.Specialized;
using FChatLib.Entities.EventHandlers;

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
        private CancellationToken _cancellationToken;
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
