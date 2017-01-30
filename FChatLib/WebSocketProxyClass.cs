using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace FChatLib
{
    [Serializable]
    public class WebSocketProxyClass : WebSocket
    {
        public WebSocketProxyClass(string url, params string[] protocols) : base(url, protocols)
        {
        }
    }
}
