﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib.Entities.Commands
{
    class SetStatus : BaseCommand
    {
        public string status;
        public string statusmsg;

        public SetStatus()
        {
            Type = "STA";
        }
    }
}
