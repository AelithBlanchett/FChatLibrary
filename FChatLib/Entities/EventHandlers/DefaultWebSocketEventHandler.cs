using FChatLib.Entities.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace FChatLib.Entities.EventHandlers
{
    class DefaultWebSocketEventHandler : BaseWebSocketEventHandler
    {

        public int DelayBetweenEachReconnection;
        private Identification _identificationInfo;

        public DefaultWebSocketEventHandler(WebSocket wsClient, Identification identificationInfo, int delayBetweenEachReconnection) : base(wsClient)
        {
            delayBetweenEachReconnection = DelayBetweenEachReconnection;
            _identificationInfo = identificationInfo;
        }

        public override void OnClose(object sender, CloseEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("WebSocket connection closed. Retyring again in 4000ms.");
            System.Threading.Thread.Sleep(DelayBetweenEachReconnection);
            WebSocketClient.Connect();
        }

        public override void OnMessage(object sender, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void OnOpen(object sender, EventArgs e)
        {
            WebSocketClient.Send(_identificationInfo.ToString());
        }
    }
}
