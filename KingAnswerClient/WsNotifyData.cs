using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingAnswerClient
{
    public interface IWsNotifyData
    {
        WsCmd Cmd { get; set; }

        string ToJson();
    }

    public class WsNotifyData<T> : IWsNotifyData
    {
        public T Data { get; set; }

        public WsCmd Cmd { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
