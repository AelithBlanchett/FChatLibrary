using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FChatLib.Entities.Events
{
    public abstract class BaseEvent : IBaseEvent
    {
        private string _type;

        [JsonIgnore]
        public string Data
        {
            get
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        [JsonIgnore]
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public override string ToString()
        {
            return $"{Type} {Data}";
        }
    }
}
