using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlacetelDialer
{
    public class PlacetelDialerConfig : ConfigurationSection
    {
        [ConfigurationProperty("apiKey")]
        public string ApiKey
        {
            get { return (String)this["apiKey"]; }
            set { this["apiKey"] = value; }
        }

        [ConfigurationProperty("sipUser")]
        public string SipUser
        {
            get { return (String)this["sipUser"]; }
            set { this["sipUser"] = value; }
        }
    }
}
