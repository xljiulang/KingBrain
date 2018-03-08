using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    /// <summary>
    /// app配置
    /// </summary>
    public static class AppConfig
    {
        /// <summary>
        /// 公网域名或ip
        /// </summary>
        public static readonly string Host = ConfigurationManager.AppSettings["Host");

        /// <summary>
        /// 代理端口
        /// </summary>
        public static readonly int ProxyPort = int.Parse(ConfigurationManager.AppSettings["ProxyPort"]);

        /// <summary>
        /// ws服务端口
        /// </summary>
        public static readonly int WsPort = int.Parse(ConfigurationManager.AppSettings["WsPort"]);

        /// <summary>
        /// 是否在响应给手机端时附加答案
        /// </summary>
        public static readonly bool ResponseAnswer = ConfigurationManager.AppSettings["ResponseAnswer"] == "true";

        /// <summary>
        /// 走代理的host
        /// </summary>
        public static readonly string[] ProxyHosts;

        /// <summary>
        /// 是否仅代理pac列表的域名
        /// </summary>
        private static readonly bool proxyPacOnly;

        /// <summary>
        /// 构造器
        /// </summary>
        static AppConfig()
        {
            var section = ConfigurationManager.GetSection("proxyConfig") as ProxyConfigSection;
            proxyPacOnly = section.ProxyPacOnly;
            ProxyHosts = section.PAC.Cast<PacItem>().Select(item => item.host).ToArray();
        }

        /// <summary>
        /// 是否允许代理
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static bool AllowProxy(string host)
        {
            if (proxyPacOnly == false)
            {
                return true;
            }
            return ProxyHosts.Any(item => item.Equals(host, StringComparison.OrdinalIgnoreCase));
        }

    }
}
