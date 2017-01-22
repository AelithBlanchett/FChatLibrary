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
using FChatLib.Entities.Messages;
using System.Collections.Specialized;

namespace FChatLib
{
    public class Bot : IBot
    {

        private string _username;
        private string _password;
        private string _botCharacterName;
        private string _administratorCharacterName;
        private bool _debug;
        private bool _reconnectOnError;
        private int _delayBetweenEachReconnection;
        private CancellationToken _cancellationToken;
        private WebSocket wsClient;
        private IWebSocketEventHandler _wsEventHandler;

        public IWebSocketEventHandler WSEventHandler
        {
            get
            {
                return _wsEventHandler;
            }

            set
            {
                _wsEventHandler = value;
            }
        }

        public Bot(string username, string password, string botCharacterName, string administratorCharacterName)
        {
            _username = username;
            _password = password;
            _botCharacterName = botCharacterName;
            _administratorCharacterName = administratorCharacterName;
            _debug = false;
            _reconnectOnError = true;
            _delayBetweenEachReconnection = 4000;
        }

        public Bot(string username, string password, string botCharacterName, string administratorCharacterName, bool debug, bool reconnectOnError, int delayBetweenEachReconnection) : this(username, password, botCharacterName, administratorCharacterName)
        {
            _debug = debug;
            _reconnectOnError = reconnectOnError;
            _delayBetweenEachReconnection = delayBetweenEachReconnection;
        }

        public Bot(string username, string password, string botCharacterName, string administratorCharacterName, bool debug, bool reconnectOnError, int delayBetweenEachReconnection, IWebSocketEventHandler eventHandler) : this(username, password, botCharacterName, administratorCharacterName, debug, reconnectOnError, delayBetweenEachReconnection)
        {
            _wsEventHandler = eventHandler;
        }

        public void Connect()
        {
            var ticket = GetTicket();


            int port = 9722;
            if (_debug == true)
            {
                port = 8722;
            }

            wsClient = new WebSocket($"ws://chat.f-list.net:{port}");

            var handler = new WebSocketEventHandler();

            wsClient.OnOpen += (sender, e) =>
            {
                var jsonData = JsonConvert.SerializeObject(new Identification()
                {
                    account = _username,
                    botVersion = "1.0.0",
                    character = _botCharacterName,
                    ticket = ticket,
                    method = "ticket",
                    botCreator = _username
                });
                wsClient.Send($"IDN {jsonData}");
            };

            wsClient.OnOpen += handler.OnOpen;
            wsClient.OnClose += handler.OnClose;
            wsClient.OnMessage += handler.OnMessage;
            wsClient.OnError += handler.OnError;

            if (_reconnectOnError)
            {
                wsClient.OnError += (sender, e) =>
                {
                    Console.WriteLine("WebSocket connection closed. Retyring again in 4000ms.");
                    System.Threading.Thread.Sleep(_delayBetweenEachReconnection);
                    Connect();
                };
            }

            wsClient.Connect();
        }

        public void Disconnect()
        {
            wsClient.Close(CloseStatusCode.Normal);
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
            return jsonObject.ticket;
        }
    }
}
