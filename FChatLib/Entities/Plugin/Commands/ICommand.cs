﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib.Entities.Plugin.Commands
{
    public interface ICommand
    {
        string Description { get; }
        string ExampleUsage { get; }
        void ExecuteCommand();
    }
}
