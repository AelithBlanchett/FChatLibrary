using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib.Entities.EventHandlers
{
    public class ReceivedPluginCommandEventArgs : EventArgs
    {
        public string Channel;
        public string Character;
        public string Command;
        public string[] Arguments;
    }
}
