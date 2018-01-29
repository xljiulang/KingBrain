using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    /// <summary>
    /// 定义ws通知内容
    /// </summary>
    public interface IWsNotifyData
    {
        /// <summary>
        /// 命令值
        /// </summary>
        WsCmd Cmd { get; set; }

        /// <summary>
        /// 转换为json
        /// </summary>
        /// <returns></returns>
        string ToJson();
    }

    /// <summary>
    /// 表示ws通知内容
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WsNotifyData<T> : IWsNotifyData
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 命令值
        /// </summary>
        public WsCmd Cmd { get; set; }

        /// <summary>
        /// 转换为json
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
