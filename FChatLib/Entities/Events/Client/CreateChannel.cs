﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib.Entities.Events.Client
{
    class CreateChannel : BaseEvent
    {
        public string channel;

        public CreateChannel()
        {
            Type = "CCR";
        }
    }
}
