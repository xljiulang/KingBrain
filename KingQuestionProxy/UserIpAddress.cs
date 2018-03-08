using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    public class UserIpAddress
    {
        public string User { get; set; }

        public string IpAddress { get; set; }

        public override string ToString()
        {
            return $"{User} {IpAddress}";
        }
    }
}
