using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib.Entities.Messages
{
    public class Identification
    {
        //{ "method": "ticket", "account": parent.config.username, "ticket": ticket, "character": parent.config.character, "cname": parent.config.cname, "cversion": parent.config.cversion };
        public string method;
        public string account;
        public string ticket;
        public string character;
        [JsonProperty("cname")]
        public string botCreator;
        [JsonProperty("cversion")]
        public string botVersion;
    }
}
