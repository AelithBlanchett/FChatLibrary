using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib.Entities.Plugin
{
    public interface IPlugin
    {
        string Name { get; }
        string Version { get; }
        IBot FChatClient { get; }
        IEnumerable<string> GetCommandList();
    }
}
