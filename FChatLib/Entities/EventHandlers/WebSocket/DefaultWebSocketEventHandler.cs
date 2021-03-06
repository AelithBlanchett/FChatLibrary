﻿using FChatLib.Entities.EventHandlers.FChatEvents;
using FChatLib.Entities.Events.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace FChatLib.Entities.EventHandlers.WebSocket
{
    public class DefaultWebSocketEventHandler : BaseWebSocketEventHandler
    {

        public int DelayBetweenEachReconnection;

        [NonSerialized]
        private Identification _identificationInfo;

        public DefaultWebSocketEventHandler(WebSocketSharp.WebSocket wsClient, Identification identificationInfo, int delayBetweenEachReconnection) : base(wsClient)
        {
            delayBetweenEachReconnection = DelayBetweenEachReconnection;
            _identificationInfo = identificationInfo;
        }

        public override void OnClose(object sender, CloseEventArgs e)
        {
            Console.WriteLine($"Closed connection. Code:  {e.Code} Reason: {e.Reason}");
        }

        public override void OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("WebSocket connection closed. Retyring again in 4000ms.");
            System.Threading.Thread.Sleep(DelayBetweenEachReconnection);
            WebSocketClient.Connect();
        }

        public override void OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Data);
            DefaultFChatEventHandler.DetectEvent(e.Data);
        }

        public override void OnOpen(object sender, EventArgs e)
        {
            WebSocketClient.Send(_identificationInfo.ToString());
        }
    }
}
