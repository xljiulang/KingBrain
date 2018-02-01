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
using System.Data.Entity;
using System.Net.NetworkInformation;

namespace KingQuestionProxy
{
    /// <summary>
    /// Http服务
    /// </summary>
    public class HomeController : HttpController
    {
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

            var proxyHost = this.GetProxyHost();
            var model = new IndexModel
            {
                ProxyIpEndpoint = $"{proxyHost}:{AppConfig.ProxyPort}",
                WsIpEndpoint = $"{proxyHost}:{AppConfig.WsPort}",
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

        /// <summary>
        /// 生成WsGameAnswer的html
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("/GetHtml")]
        public ActionResult GetHtml([Body] WsGameAnswer model)
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
            var disposition = "attachment;filename=data.db";
            return File(SqlliteContext.DbFile, "application/octet-stream", disposition);
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <returns></returns>
        [Route("/Data/Import")]
        public async Task<ActionResult> ImportDataAsync()
        {
            if (this.Request.Files.Length == 0)
            {
                return this.RedirectToIndex();
            }
            var file = this.Request.Files.First();
            if (file.FileName.EndsWith(".db", StringComparison.OrdinalIgnoreCase) == false)
            {
                return this.RedirectToIndex();
            }

            var dbFile = "data\\import.db";
            System.IO.File.WriteAllBytes(dbFile, file.Stream);

            using (var sourceDb = new SqlliteContext(dbFile))
            {
                using (var targetDb = new SqlliteContext())
                {
                    var datas = await sourceDb.QuizAnswer.ToArrayAsync();
                    foreach (var data in datas)
                    {
                        if (await targetDb.QuizAnswer.AnyAsync(item => item.Quiz == data.Quiz) == false)
                        {
                            targetDb.QuizAnswer.Add(data);
                        }
                    }
                    await targetDb.SaveChangesAsync();
                }
            }

            System.IO.File.Delete(dbFile);
            return this.RedirectToIndex();
        }

        /// <summary>
        /// 动态生成pac
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
            this.Response.ContentType = "application/x-ns-proxy-autoconfig";
            return Content(pacString);
        }

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
        /// 中转到首页
        /// </summary>
        /// <returns></returns>
        private ActionResult RedirectToIndex()
        {
            this.Response.Status = 301;
            this.Response.Headers.Add("location", "/");
            return new EmptyResult();
        }

        protected override void OnException(NetworkSocket.Http.ExceptionContext filterContext)
        {
            Console.WriteLine(filterContext.Exception);
            base.OnException(filterContext);
        }

        /// <summary>
        /// 返回代理服务器绝对域名
        /// </summary>
        /// <returns></returns>
        private string GetProxyHost()
        {
            var ipArray = from i in NetworkInterface.GetAllNetworkInterfaces()
                          where i.OperationalStatus == OperationalStatus.Up
                          let address = i.GetIPProperties().UnicastAddresses
                          select address.ToArray();

            var networkIps = from ip in ipArray.SelectMany(item => item)
                             where ip.DuplicateAddressDetectionState == DuplicateAddressDetectionState.Preferred
                             where ip.PrefixOrigin == PrefixOrigin.Dhcp || ip.PrefixOrigin == PrefixOrigin.Manual
                             where ip.SuffixOrigin == SuffixOrigin.OriginDhcp || ip.SuffixOrigin == SuffixOrigin.Manual
                             select ip;

            var networkIp = networkIps.FirstOrDefault();
            return networkIp == null ? this.Request.Url.Host : networkIp.Address.ToString();
        }
    }
}
