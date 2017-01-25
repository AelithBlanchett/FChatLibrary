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
            bot.JoinChannel("ADH-92a9bd86405869c8a768");
            Console.ReadLine();
            bot.Plugins.LoadPlugin("test", "ADH-92a9bd86405869c8a768");
            Console.ReadLine();
        }
    }
}
