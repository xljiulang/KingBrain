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
        /// 代理端口
        /// </summary>
        public static readonly int ProxyPort = int.Parse(ConfigurationManager.AppSettings["ProxyPort"]);

        /// <summary>
        /// ws服务端口
        /// </summary>
        public static readonly int WsPort = int.Parse(ConfigurationManager.AppSettings["WsPort"]);

        /// <summary>
        /// 走代理的host
        /// </summary>
        public static readonly string[] ProxyHosts;

        /// <summary>
        /// 构造器
        /// </summary>
        static AppConfig()
        {
            var section = ConfigurationManager.GetSection("PacConfig") as PacConfigSection;
            ProxyHosts = section.proxy.Cast<PacItem>().Select(item => item.host).ToArray();
        }

        /// <summary>
        /// 是否允许代理
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static bool AllowProxy(string host)
        {
            return ProxyHosts.Any(item => item.Equals(host, StringComparison.OrdinalIgnoreCase));
        }

    }
}
