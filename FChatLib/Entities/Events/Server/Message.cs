﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib.Entities.Events.Server
{
    public class Message : BaseEvent
    {
        public string character;
        public string message;
        public string channel;

        public Message()
        {
            this.Type = "MSG";
        }
    }
}
