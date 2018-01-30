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
using System.Diagnostics;

namespace KingQuestionProxy
{
    /// <summary>
    /// 王者数据处理器
    /// </summary>
    static class KingProcesser
    {
        /// <summary>
        /// http和ws监听器
        /// </summary>
        private static readonly TcpListener listener = new TcpListener();

        /// <summary>
        /// http和ws商品
        /// </summary>
        private static readonly int wsPort = int.Parse(ConfigurationManager.AppSettings["WsPort"]);

        /// <summary>
        /// 是否回复答案
        /// </summary>
        private static readonly bool withAnswer = ConfigurationManager.AppSettings["WithAnswer"] == "true";

        /// <summary>
        /// 王者数据处理器
        /// </summary>
        static KingProcesser()
        {
            var http = listener.Use<HttpMiddleware>();
            http.MIMECollection.Add(".cer", "application/x-x509-ca-cert");

            listener.Use<WebsocketMiddleware>();
            listener.Start(wsPort);
        }

        /// <summary>
        /// 显式初始化
        /// </summary>
        public static void Init()
        {
        }

        /// <summary>
        /// 关闭监听器
        /// </summary>
        public static void CloseListener()
        {
            listener.Dispose();
        }

        /// <summary>
        /// 处理会话
        /// </summary>
        /// <param name="session">会话</param>
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
                HistoryDataTable.Save();
            }
        }

        /// <summary>
        /// 从本地和网络查找答案
        /// 并转发给对应的ws客户端
        /// </summary>
        /// <param name="session">会话</param>
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

            var beginTime = DateTime.Now;
            var data = await SearchHistoryDataAsync(session, withAnswer);


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


            if (withAnswer == true)
            {
                var qData = data.QuestionData;
                if (data.SearchResult.Best != null)
                {
                    var index = data.SearchResult.Best.Index;
                    qData.quiz = qData.quiz + $" [{(char)('A' + index)}]";
                    qData.options[index] = qData.options[index] + "[√]";
                }

                var q = new KingQuestion { data = qData };
                var json = JsonConvert.SerializeObject(q);
                session.utilSetResponseBody(json);
            }
        }

        /// <summary>
        /// 从本地和网络查找答案
        /// 并转发给对应的ws客户端
        /// </summary>
        /// <param name="session">会话</param>
        /// <param name="sync">是否同步</param>
        /// <returns></returns>
        private static async Task<HistoryData> SearchHistoryDataAsync(Session session, bool sync)
        {
            session.utilDecodeRequest();
            session.utilDecodeResponse();

            var requestBody = Encoding.UTF8.GetString(session.RequestBody);
            var responseBody = Encoding.UTF8.GetString(session.ResponseBody);

            var kingQuestion = KingQuestion.Parse(responseBody);
            if (kingQuestion == null || kingQuestion.IsValidate() == false)
            {
                return null;
            }

            var kingRequest = KingRequest.Parse(requestBody);
            var title = kingQuestion.data.quiz;
            var beginTime = DateTime.Now;
            var history = HistoryDataTable.TryGet(title);

            if (history == null)
            {
                history = new HistoryData
                {
                    QuestionData = kingQuestion.data,
                    KingRequest = KingRequest.Parse(requestBody),
                    SearchResult = await BaiduSearcher.SearchAsync(kingQuestion, withAnswer)
                };
                HistoryDataTable.TryAdd(history);
            }

            var data = JsonConvert.DeserializeObject<HistoryData>(JsonConvert.SerializeObject(history));
            data.KingRequest = kingRequest;
            data.QuestionData = kingQuestion.data;
            data.SearchResult = history.SearchResult.CreateNewByQuestionData(kingQuestion.data);
            return data;
        }

        /// <summary>
        /// 发送答案给ws客户端
        /// </summary>
        /// <param name="notifyData">数据内容</param>
        /// <param name="clientIp">客户端ip</param>
        private static void WsNotifyByClientIP(IWsNotifyData notifyData, string clientIp)
        {
            var jsonResult = notifyData.ToJson();
            var wsSessions = listener.SessionManager.FilterWrappers<WebSocketSession>();

            foreach (var ws in wsSessions)
            {
                try
                {
                    var ip = ws.Tag.Get("ip").ToString();
                    Debugger.Log(0, null, $"转发数据到{ip}");
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

        /// <summary>
        /// 更新最佳选项并保存
        /// </summary>
        /// <param name="session"></param>
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
