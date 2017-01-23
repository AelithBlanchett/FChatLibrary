using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib_tst
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new FChatLib.Bot("dollydolly", "", "Xo Gisele", "Xo Gisele", true, 4000);
            bot.Connect();
            Console.ReadLine();
            var objs = bot.LoadPluginsFromAssembly("FChatLib.Plugin.Test");
            foreach (dynamic plugin in objs)
            {
                Console.WriteLine($"Loaded plugin {plugin.Name} v{plugin.Version}");
            }
            Console.ReadLine();
        }
    }
}
