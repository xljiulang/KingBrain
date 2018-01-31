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
    public class FiddlerApp : ServiceControl
    {
        /// <summary>
        /// 所有会话的集合
        /// </summary>
        public static readonly SessionCollection AllSessions = new SessionCollection();

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
                Console.WriteLine($"{session.clientIP} {session.fullUrl}");
                session.bBufferResponse = true;

                // 首页重定向
                var uri = new Uri(session.fullUrl);
                if (uri.Port == AppConfig.ProxyPort || uri.Port == AppConfig.WsPort)
                {
                    session.host = $"{uri.Host}:{ AppConfig.WsPort}";
                    AllSessions.Add(session);
                }
                else if (AppConfig.AllowProxy(uri.Host))
                {
                    AllSessions.Add(session);
                }
                else
                {
                    session.Abort();
                }
            };

            // 收到服务端的回复
            FiddlerApplication.BeforeResponse += (session) =>
            {
                KingProcesser.ProcessSession(session);
            };


            // 配置代理服务器
            CONFIG.IgnoreServerCertErrors = true;
            FiddlerApp.SetRootCertificate();

            FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.abortifclientaborts", true);
            FiddlerApplication.Startup(AppConfig.ProxyPort, FiddlerCoreStartupFlags.AllowRemoteClients | FiddlerCoreStartupFlags.DecryptSSL);

            return true;
        }

        /// <summary>
        /// 设置证书
        /// </summary>
        private static void SetRootCertificate()
        {
            if (Cert.Exists() == false)
            {
                var certString = File.ReadAllText(Cert.Proxy_Cert, Encoding.ASCII);
                var keyString = File.ReadAllText(Cert.Proxy_Key, Encoding.ASCII);

                FiddlerApplication.Prefs.SetStringPref("fiddler.certmaker.bc.cert", certString);
                FiddlerApplication.Prefs.SetStringPref("fiddler.certmaker.bc.key", keyString);
            }
            else
            {
                CertMaker.createRootCert();
                var cert = CertMaker.GetRootCertificate();

                var clientCer = cert.Export(X509ContentType.Cert);
                File.WriteAllBytes(Cert.Client_Cer, clientCer);

                var certString = FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.bc.cert", null);
                var keyString = FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.bc.key", null);

                File.WriteAllText(Cert.Proxy_Cert, certString, Encoding.ASCII);
                File.WriteAllText(Cert.Proxy_Key, keyString, Encoding.ASCII);
            }
        }

        /// <summary>
        /// 停止服务 
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public bool Stop(HostControl hostControl)
        {
            HistoryDataTable.Save();
            KingProcesser.CloseListener();
            FiddlerApplication.Shutdown();
            return true;
        }
    }
}
