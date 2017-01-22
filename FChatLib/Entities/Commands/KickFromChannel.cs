using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib.Entities.Commands
{
    class KickFromChannel : BaseCommand
    {
        public string channel;
        public string character;

        public KickFromChannel()
        {
            Type = "CKU";
        }
    }
}
