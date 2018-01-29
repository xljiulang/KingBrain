using Fiddler;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Configuration;
using WebApiClient;
using System.Net.Http;
using Topshelf;
using System.Diagnostics;

namespace KingQuestionProxy
{
    /// <summary>
    /// 表示Fiddler应用
    /// </summary>
    public class FiddlerApp : Topshelf.ServiceControl
    {
        /// <summary>
        /// 代理端口
        /// </summary>
        private static readonly int proxyPort = int.Parse(ConfigurationManager.AppSettings["ProxyPort"]);

        /// <summary>
        /// SSL代理中转服务地址
        /// </summary>
        private static readonly Uri sslAddress = new Uri(ConfigurationManager.AppSettings["SslProxyAddress"]);

        /// <summary>
        /// SSL代理服务
        /// </summary>
        private static Proxy sslProxyServer;

        /// <summary>
        /// 停止服务 
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Stop(HostControl hostControl)
        {
            HistoryDataTable.Save();
            KingProcesser.CloseListener();
            Debugger.Log(0, null, "Save HistoryDataTable Datas OK ..");

            if (sslProxyServer != null)
            {
                sslProxyServer.Dispose();
            }
            FiddlerApplication.Shutdown();
            return true;
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Start(HostControl hostControl)
        {
            KingProcesser.Init();
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            FiddlerApplication.BeforeRequest += (session) =>
            {
                session.bBufferResponse = false;
                if ((session.oRequest.pipeClient.LocalPort == sslAddress.Port) && (session.hostname == sslAddress.Host))
                {
                    session.utilCreateResponseAndBypassServer();
                    session.oResponse.headers.SetStatus(200, "Ok");
                    session.utilSetResponseBody("SSL Proxy OK ..");
                }
            };

            // 收到服务端的回复
            FiddlerApplication.BeforeResponse += (session) =>
            {
                KingProcesser.ProcessSessionAsync(session);
            };

            // 配置代理服务器
            CONFIG.IgnoreServerCertErrors = true;
            FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.abortifclientaborts", true);
            FiddlerApplication.Startup(proxyPort, false, true, true);

            sslProxyServer = FiddlerApplication.CreateProxyEndpoint(sslAddress.Port, true, sslAddress.Host);
            if (sslProxyServer == null)
            {
                Console.WriteLine("创建SSL监听失败");
            }
            return true;
        }
    }
}
