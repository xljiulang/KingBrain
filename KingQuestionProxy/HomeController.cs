using NetworkSocket.Http;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    /// <summary>
    /// Http服务
    /// </summary>
    public class HomeController : HttpController
    {
        /// <summary>
        /// 代理端口
        /// </summary>
        private static readonly string proxyPort = ConfigurationManager.AppSettings["ProxyPort"];

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [Route("/")]
        public ActionResult Index()
        {
            var model = new IndexModel
            {
                cerhref = $"http://{this.Request.Url.Host}:{proxyPort}/fiddlerRoot.cer",
                proxyIpEndpoint = $"{this.Request.Url.Host}:{proxyPort}",
                clientIpAddress = (this.Request.RemoteEndPoint as IPEndPoint).Address.ToString()
            };

            var html = System.IO.File.ReadAllText("index.html", Encoding.UTF8)
                   .Replace("@cerhref", model.cerhref)
                   .Replace("@proxyIpEndpoint", model.proxyIpEndpoint)
                   .Replace("@clientIpAddress", model.clientIpAddress);

            return Content(html);
        }
    }
}
