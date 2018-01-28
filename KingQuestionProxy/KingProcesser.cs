using Fiddler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkSocket;
using NetworkSocket.Http;
using NetworkSocket.WebSocket;
using Newtonsoft.Json;
using System.Net;
using System.Configuration;

namespace KingQuestionProxy
{
    static class KingProcesser
    {
        private static readonly TcpListener listener = new TcpListener();

        private static readonly int wsPort = int.Parse(ConfigurationManager.AppSettings["WsPort"]);


        static KingProcesser()
        {
            listener.Use<HttpMiddleware>();
            listener.Use<WebsocketMiddleware>();
            listener.Start(wsPort);
        }

        public static void Init()
        {
        }

        public static void CloseListener()
        {
            listener.Dispose();
        }

        /// <summary>
        /// 用于接收代理服务器转发的数据
        /// </summary>
        /// <param name="requestUrl">请求地址</param>
        /// <returns></returns>

        public static async void ProcessSessionAsync(Session session)
        {
            var url = session.fullUrl;
            if (url.Contains("question/bat/findQuiz") == true)
            {
                await SearchAnswerAsync(session);
            }
            else if (url.Contains("question/bat/choose") == true)
            {
                UpdateBestAndSave(session);
            }
            else if (url.Contains("question/bat/fightResult") == true)
            {
                var notifyData = new WsNotifyData<object> { Cmd = WsCmd.GameOver };
                WsNotifyByClientIP(notifyData, session.clientIP);
            }
        }

        /// <summary>
        /// 找答案并打印
        /// </summary>
        /// <param name="question">问题</param>
        /// <returns></returns>
        private static async Task SearchAnswerAsync(Session session)
        {
            session.utilDecodeRequest();
            session.utilDecodeResponse();

            var requestBody = Encoding.UTF8.GetString(session.RequestBody);
            var responseBody = Encoding.UTF8.GetString(session.ResponseBody);

            var kingQuestion = KingQuestion.Parse(responseBody);
            if (kingQuestion == null || kingQuestion.IsValidate() == false)
            {
                return;
            }

            var title = kingQuestion.data.quiz;
            var beginTime = DateTime.Now;
            var data = HistoryDataTable.TryGet(title);
            if (data == null)
            {
                var searchResult = await BaiduSearcher.SearchAsync(kingQuestion);
                data = new HistoryData
                {
                    SearchResult = searchResult,
                    QuestionData = kingQuestion.data,
                    KingRequest = KingRequest.Parse(requestBody)
                };
                HistoryDataTable.TryAdd(data);
            }

            const double offsetSecondes = 3.7d;
            var delay = beginTime.AddSeconds(offsetSecondes).Subtract(DateTime.Now);

            var notifyData = new WsNotifyData<WsGameAnswer>
            {
                Cmd = WsCmd.GameAnser,
                Data = new WsGameAnswer
                {
                    SearchResult = data.SearchResult,
                    GameDelayMSeconds = (int)delay.TotalMilliseconds
                }
            };
            WsNotifyByClientIP(notifyData, session.clientIP);
        }

        private static void WsNotifyByClientIP(IWsNotifyData notifyData, string clientIp)
        {
            var jsonResult = notifyData.ToJson();
            var wsSessions = listener.SessionManager.FilterWrappers<WebSocketSession>();

            foreach (var ws in wsSessions)
            {
                try
                {
                    var ip = ws.Tag.Get("ip").ToString();
                    Console.WriteLine(ip);

                    if (clientIp == ip || clientIp.Contains(ip))
                    {
                        ws.SendText(jsonResult);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void UpdateBestAndSave(Session session)
        {
            session.utilDecodeRequest();
            session.utilDecodeResponse();

            var requestBody = Encoding.UTF8.GetString(session.RequestBody);
            var responseBody = Encoding.UTF8.GetString(session.ResponseBody);

            var kingRequest = KingRequest.Parse(requestBody);
            var kingAnswer = KingAnswer.Parse(responseBody);
            if (kingAnswer != null && kingAnswer.data != null)
            {
                var data = HistoryDataTable.TryGet(kingRequest);
                if (data != null)
                {
                    var index = kingAnswer.data.answer - 1;
                    data.SearchResult.Best = data.SearchResult.Options[index];
                }
            }
        }
    }
}
