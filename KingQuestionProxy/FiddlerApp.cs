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

            FiddlerApplication.BeforeRequest += (session) =>
            {
                session.bBufferResponse = true;
                AllSessions.Add(session);
            };

            // 收到服务端的回复
            FiddlerApplication.BeforeResponse += (session) =>
            {
                KingProcesser.ProcessSession(session);
            };

            var e = CertMaker.rootCertExists();
            var r = CertMaker.GetRootCertificate();

            // 配置代理服务器
            CONFIG.IgnoreServerCertErrors = true;
            FiddlerApp.SetRootCertificate();

            FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.abortifclientaborts", true);
            FiddlerApplication.Startup(proxyPort, FiddlerCoreStartupFlags.AllowRemoteClients | FiddlerCoreStartupFlags.DecryptSSL);

            return true;
        }

        /// <summary>
        /// 设置证书
        /// </summary>
        private static void SetRootCertificate()
        {
            if (File.Exists("proxy.cer") && File.Exists("proxy.key"))
            {
                var certString = File.ReadAllText("proxy.cer", Encoding.ASCII);
                var keyString = File.ReadAllText("proxy.key", Encoding.ASCII);

                FiddlerApplication.Prefs.SetStringPref("fiddler.certmaker.bc.cert", certString);
                FiddlerApplication.Prefs.SetStringPref("fiddler.certmaker.bc.key", keyString);
            }
            else
            {
                var cert = CertMaker.GetRootCertificate();
                var clientCer = cert.Export(X509ContentType.Cert);
                File.WriteAllBytes("client.cer", clientCer);

                var certString = FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.bc.cert", null);
                var keyString = FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.bc.key", null);

                File.WriteAllText("proxy.cer", certString, Encoding.ASCII);
                File.WriteAllText("proxy.key", keyString, Encoding.ASCII);
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
