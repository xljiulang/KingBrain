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
using System.Net;

namespace KingQuestionProxy
{
    /// <summary>
    /// 表示Fiddler应用
    /// </summary>
    public class FiddlerApp : ServiceControl
    {
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Start(HostControl hostControl)
        {
            KingProcesser.Init();
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            // 请求前
            FiddlerApplication.BeforeRequest += (session) =>
            {
                session.bBufferResponse = true;                
                var uri = new Uri(session.fullUrl);

                if (uri.Port == AppConfig.ProxyPort || uri.Port == AppConfig.WsPort)
                {
                    session.RequestHeaders["ClientIpAddress"] = session.clientIP;
                    session.host = $"{uri.Host}:{ AppConfig.WsPort}";
                }
                else if (AppConfig.AllowProxy(uri.Host) == false)
                {
                    session.Abort();
                    Console.WriteLine($"拒绝代理：{session.clientIP}->{session.fullUrl}");
                }
                else
                {
                    Console.WriteLine($"转发代理：{session.clientIP}->{session.fullUrl}");
                }
            };

            // 收到服务端的回复
            FiddlerApplication.BeforeResponse += (session) =>
            {
                try
                {
                    KingProcesser.ProcessSession(session);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            };

            // 配置代理服务器
            Cert.SetRootCertificate();
            CONFIG.IgnoreServerCertErrors = true;

            FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.abortifclientaborts", true);
            FiddlerApplication.Startup(AppConfig.ProxyPort, FiddlerCoreStartupFlags.AllowRemoteClients | FiddlerCoreStartupFlags.DecryptSSL);

            return true;
        }



        /// <summary>
        /// 停止服务 
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Stop(HostControl hostControl)
        {
            KingProcesser.CloseListener();
            FiddlerApplication.Shutdown();
            return true;
        }
    }
}
