using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    public class IndexModel
    {
        public string ProxyIpEndpoint { get; set; }

        public string WsIpEndpoint { get; set; }

        public string[] ClientsIp { get; set; }
    }
}
