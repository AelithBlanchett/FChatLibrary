using System;
using WebSocketSharp;

namespace FChatLib
{
    public interface IWebSocketEventHandler
    {
        WebSocket WebSocketClient { get; set; }
        void OnOpen(object sender, EventArgs e);
        void OnClose(object sender, CloseEventArgs e);
        void OnError(object sender, ErrorEventArgs e);
        void OnMessage(object sender, MessageEventArgs e);
    }
}