using FChatLib.Entities.Plugin.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib.Entities.Plugin
{
    [Serializable]
    public abstract class BasePlugin : MarshalByRefObject, IPlugin
    {
        public IBot FChatClient { get; set; }
        public abstract string Name { get; }
        public abstract string Version { get; }
        public string Channel { get; set; }

        public BasePlugin(IBot bot, string channel)
        {
            FChatClient = bot;
            Channel = channel;
        }

        public virtual List<string> GetCommandList()
        {
            var type = typeof(ICommand);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ICommand).IsAssignableFrom(p));
            var listOfTypes = types.Select(x => x.Name).Where(x => x != "ICommand" && x != "BaseCommand").Distinct();
            return listOfTypes.ToList();
        }

        public bool DoesCommandExist(string command)
        {
            var commandList = GetCommandList();
            return (commandList.FirstOrDefault(x => x == command) != null);
        }

        public bool ExecuteCommand(string command, string[] args)
        {
            if (DoesCommandExist(command))
            {
                var searchedType = typeof(ICommand);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(s => s.GetTypes())
                            .Where(p => typeof(ICommand).IsAssignableFrom(p));
                var typeToCreate= types.FirstOrDefault(x => x.Name == command);
                if(typeToCreate != null)
                {
                    ICommand instance = (ICommand)Activator.CreateInstance(typeToCreate, this);
                    instance.ExecuteCommand();
                }
                return true;
            }
            return false;
        }
    }
}
