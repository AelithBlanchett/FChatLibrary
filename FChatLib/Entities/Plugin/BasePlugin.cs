using FChatLib.Entities.Plugin.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib.Entities.Plugin
{
    public abstract class BasePlugin : IPlugin
    {
        public abstract IBot FChatClient { get; }
        public abstract string Name { get; }
        public abstract string Version { get; }

        public IEnumerable<string> GetCommandList()
        {
            var type = typeof(ICommand);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));
            var listOfTypes = types.Select(x => x.Name);
            return listOfTypes;
        }

        public bool DoesCommandExist(string command)
        {
            return (this.GetCommandList().SingleOrDefault(x => x == command) != null);
        }

        public bool ExecuteCommand(string command)
        {
            if (DoesCommandExist(command))
            {
                var searchedType = typeof(ICommand);
                var typeToCreate = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => searchedType.IsAssignableFrom(p))
                .SingleOrDefault(x => x.Name == command);
                if(typeToCreate != null)
                {
                    ICommand instance = (ICommand)Activator.CreateInstance(typeToCreate, this);
                    instance.ExecuteCommand();
                }
                
            }
            return (this.GetCommandList().SingleOrDefault(x => x == command) != null);
        }
    }
}
