using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib.Entities.Commands
{
    class CreateChannel : BaseCommand
    {
        public string channel;

        public CreateChannel()
        {
            Type = "CCR";
        }
    }
}
