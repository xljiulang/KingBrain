using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    public static class Cert
    {
        public const string Client_Cer = "Cert\\client.cer";

        public const string Proxy_Cert = "Cert\\proxy.cer";

        public const string Proxy_Key = "Cert\\proxy.key";


        public static bool Exists()
        {
            return File.Exists(Client_Cer) && File.Exists(Proxy_Cert) && File.Exists(Proxy_Key);
        }
    }
}
