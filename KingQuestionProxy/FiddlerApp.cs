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
using KingQuestion;
using WebApiClient;
using System.Net.Http;
using Topshelf;

namespace KingQuestionProxy
{
    public class FiddlerApp : Topshelf.ServiceControl
    {
        private static readonly int proxyPort = int.Parse(ConfigurationManager.AppSettings["ProxyPort"]);

        private static readonly Uri sslAddress = new Uri(ConfigurationManager.AppSettings["SslProxyAddress"]);

        private static readonly string answerServer = ConfigurationManager.AppSettings["AnswerService"];

        private static readonly IAnswerApi answerApi = HttpApiClient.Create<IAnswerApi>(answerServer);

        private static Proxy sslProxyServer;

        public bool Stop(HostControl hostControl)
        {
            if (sslProxyServer != null)
            {
                sslProxyServer.Dispose();
            }
            FiddlerApplication.Shutdown();
            return true;
        }

        public bool Start(HostControl hostControl)
        {
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
            FiddlerApplication.BeforeResponse += async (session) =>
            {
                session.utilDecodeResponse();
                var content = new ByteArrayContent(session.responseBodyBytes);

                await answerApi.SendQuestionAsync(session.fullUrl, content)
                    .HandleAsDefaultWhenException(ex => Console.WriteLine(ex.Message));
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
