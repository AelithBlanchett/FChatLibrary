﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib.Entities.Events.Client
{
    class JoinChannel : BaseEvent
    {
        public string channel;

        public JoinChannel()
        {
            this.Type = "JCH";
        }
    }
}
