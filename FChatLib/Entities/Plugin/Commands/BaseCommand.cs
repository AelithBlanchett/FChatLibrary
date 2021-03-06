﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib.Entities.Plugin.Commands
{
    public abstract class BaseCommand : ICommand
    {
        public abstract string Description { get; }
        public abstract string ExampleUsage { get; }
        public abstract BasePlugin MyPlugin { get; set; }

        public abstract void ExecuteCommand();

        public BaseCommand(BasePlugin plugin)
        {
            MyPlugin = plugin;
        }
    }
}
