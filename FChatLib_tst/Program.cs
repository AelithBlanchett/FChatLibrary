﻿using System;
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
            bot.JoinChannel("ADH-55bc847c492faf154406");
            bot.Plugins.LoadPlugin("test", "ADH-55bc847c492faf154406");
            Console.ReadLine();
            bot.Plugins.ReloadPluginGlobal("test");
            Console.ReadLine();
            //bot.Plugins.UnloadPlugin("test");
            //Console.ReadLine();
            //bot.Plugins.UpdateAllPlugins();
            //Console.ReadLine();
            //bot.Plugins.ReloadPluginInChannel("test", "ADH-92a9bd86405869c8a768");
            //Console.ReadLine();
        }
    }
}
