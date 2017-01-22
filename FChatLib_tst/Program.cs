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
            var bot = new FChatLib.Bot("", "", " ", "");
            bot.Connect();
            Console.ReadLine();
        }
    }
}
