using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace FChatLib
{
    public class WebSocketEventHandler : IWebSocketEventHandler
    {
        

        public void OnOpen(object sender, EventArgs e)
        {

        }

        public void OnClose(object sender, CloseEventArgs e)
        {

        }

        public void OnError(object sender, ErrorEventArgs e)
        {

        }

        public void OnMessage(object sender, MessageEventArgs e)
        {
            //Console.WriteLine(sender.GetType().ToString());
            Console.WriteLine(e.Data);
        }

        
    }
}
