using NetworkSocket.Http;
using NetworkSocket.WebSocket;
using RazorEngine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
            var clientsIp = from session in FiddlerApp.AllSessions
                            let ip = Regex.Match(session.clientIP, @"\d+\.\d+\.\d+\.\d+").Value
                            where string.IsNullOrEmpty(ip) == false
                            select ip;

            var model = new IndexModel
            {
                CerHref = $"http://{this.Request.Url.Host}:{proxyPort}/fiddlerRoot.cer",
                ProxyIpEndpoint = $"{this.Request.Url.Host}:{proxyPort}",
                ClientsIp = clientsIp.Distinct().ToArray()
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

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/Data/Export")]
        public ActionResult ExportData()
        {
            return File("data.json", "application/octet-stream");
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <returns></returns>
        [Route("/Data/Import")]
        public ActionResult ImportData()
        {
            if (this.Request.Files.Length > 0)
            {
                var file = this.Request.Files.First();
                if (file.FileName.EndsWith("data.json", StringComparison.OrdinalIgnoreCase))
                { 
                    HistoryDataTable.TryImport(file);
                }
            }

            this.Response.Status = 301;
            this.Response.Headers.Add("location", "/");
            return new EmptyResult();
        }
    }
}
