using NetworkSocket.Http;
using NetworkSocket.WebSocket;
using RazorEngine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
        /// 返回视图
        /// </summary>
        /// <param name="name"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private ActionResult View<T>(string name, T model)
        {
            var controller = this.CurrentContext.Action.ControllerName;
            var cshtml = System.IO.File.ReadAllText($"Http\\View\\{controller}\\{name}.cshtml", Encoding.UTF8);
            var html = Razor.Parse(cshtml, model);
            return Content(html);
        }

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
                ProxyIpEndpoint = $"{this.Request.Url.Host}:{AppConfig.ProxyPort}",
                ClientsIp = clientsIp.Distinct().ToArray()
            };

            return this.View("Index", model);
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
                WsServer = $"ws://{this.Request.Url.Host}:{AppConfig.WsPort}"
            };

            return this.View("Client", model);
        }

        [Route("/GetHtml")]
        public ActionResult GetHtml([Body] SearchResult model)
        {
            return this.View("GetHtml", model);
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/Data/Export")]
        public ActionResult ExportData()
        {
            var disposition = $"attachment;filename={ Path.GetFileName(HistoryDataTable.DataFile)}";
            return File(HistoryDataTable.DataFile, "application/octet-stream", disposition);
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

        /// <summary>
        /// 获取pac
        /// </summary>
        /// <returns></returns>
        [Route("/Proxy.PAC")]
        public ActionResult Proxy_PAC()
        {
            var buidler = new StringBuilder();
            buidler.AppendLine("function FindProxyForURL(url, host){");
            buidler.AppendLine($"    var proxy = 'PROXY {this.Request.Url.Host}:{AppConfig.ProxyPort}';");
            foreach (var host in AppConfig.ProxyHosts)
            {
                buidler.AppendLine($"    if (dnsDomainIs(host, '{host}')) return proxy;");
            }
            buidler.AppendLine("    return 'DIRECT';");
            buidler.AppendLine("}");

            var pacString = buidler.ToString();
            return Content(pacString);
        }

        protected override void OnException(NetworkSocket.Http.ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }
    }
}
