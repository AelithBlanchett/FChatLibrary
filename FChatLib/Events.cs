using FChatLib.Entities.EventHandlers;
using FChatLib.Entities.EventHandlers.FChatEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib
{
    public class Events : MarshalByRefObject
    {
        public event EventHandler ReceivedFChatEvent
        {
            add { DefaultFChatEventHandler.ReceivedFChatEvent += value; }
            remove { DefaultFChatEventHandler.ReceivedFChatEvent -= value; }
        }

        public event EventHandler<ReceivedPluginCommandEventArgs> ReceivedPluginCommand
        {
            add { DefaultFChatEventHandler.ReceivedPluginCommandEvent += value; }
            remove { DefaultFChatEventHandler.ReceivedPluginCommandEvent -= value; }
        }




    }
}
