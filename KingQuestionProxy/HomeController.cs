using NetworkSocket.Http;
using RazorEngine;
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
        /// ws服务端口
        /// </summary>
        private static readonly string WsPort = ConfigurationManager.AppSettings["WsPort"];

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [Route("/")]
        public ActionResult Index()
        {
            var model = new IndexModel
            {
                CerHref = $"http://{this.Request.Url.Host}:{proxyPort}/fiddlerRoot.cer",
                ProxyIpEndpoint = $"{this.Request.Url.Host}:{proxyPort}"
            };

            var cshtml = System.IO.File.ReadAllText("View_Index.cshtml", Encoding.UTF8);
            var html = Razor.Parse(cshtml, model);
            return Content(html);
        }


        /// <summary>
        /// ws客户端页面
        /// </summary>
        /// <returns></returns>
        [Route("/client")]
        public ActionResult Client(string ip)
        {
            var model = new ClientModel
            {
                IpAddress = ip,
                WsServer = $"ws://{this.Request.Url.Host}:{WsPort}"
            };

            var cshtml = System.IO.File.ReadAllText("View_Client.cshtml", Encoding.UTF8);
            var html = Razor.Parse(cshtml, model);
            return Content(html);
        }

        [Route("/GetHtml")]
        public ActionResult GetHtml([Body] SearchResult model)
        {
            var cshtml = System.IO.File.ReadAllText("View_GetHtml.cshtml", Encoding.UTF8);
            var html = Razor.Parse(cshtml, model);
            return Content(html);
        }
    }
}
