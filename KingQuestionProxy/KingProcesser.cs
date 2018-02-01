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
        /// 王者数据处理器
        /// </summary>
        static KingProcesser()
        {
            var http = listener.Use<HttpMiddleware>();
            http.MIMECollection.Add(".cer", "application/x-x509-ca-cert");

            listener.Use<WebsocketMiddleware>();
            listener.Start(AppConfig.WsPort);
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

        public static void ProcessSession(Session session)
        {
            var url = session.fullUrl;
            if (url.Contains("question/bat/findQuiz") == true)
            {
                SetResponseWithAnswer(session);
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
        /// 从本地和网络查找答案
        /// 并转发给对应的ws客户端
        /// </summary>
        /// <param name="session">会话</param>
        private static void SetResponseWithAnswer(Session session)
        {
            var beginTime = DateTime.Now;
            var optionIndex = GetOptionIndex(session, out KingQuestion kingQuestion);
            if (kingQuestion == null)
            {
                return;
            }

            // 推送答案给ws客户端
            const double offsetSecondes = 3.7d;
            var delay = (int)beginTime.AddSeconds(offsetSecondes).Subtract(DateTime.Now).TotalMilliseconds;
            var gameAnswer = new WsGameAnswer
            {
                Index = optionIndex,
                Quiz = kingQuestion.data.quiz,
                Options = kingQuestion.data.options,
                DelayMilliseconds = delay
            };
            var notifyData = new WsNotifyData<WsGameAnswer>
            {
                Cmd = WsCmd.GameAnser,
                Data = gameAnswer,
            };
            WsNotifyByClientIP(notifyData, session.clientIP);


            // 改写响应结果
            if (optionIndex > -1)
            {
                var quizData = kingQuestion.data;
                quizData.quiz = quizData.quiz + $" [{(char)('A' + optionIndex)}]";
                quizData.options[optionIndex] = quizData.options[optionIndex] + " [√]";

                var json = JsonConvert.SerializeObject(kingQuestion);
                session.utilSetResponseBody(json);
            }
        }

        /// <summary>
        /// 从本地和网络查找答案
        /// 返回正确选项的索引
        /// </summary>
        /// <param name="session">会话</param>
        /// <returns></returns>
        private static int GetOptionIndex(Session session, out KingQuestion kingQuestion)
        {
            session.utilDecodeRequest();
            session.utilDecodeResponse();

            var requestBody = Encoding.UTF8.GetString(session.RequestBody);
            var responseBody = Encoding.UTF8.GetString(session.ResponseBody);

            kingQuestion = KingQuestion.Parse(responseBody);
            if (kingQuestion == null || kingQuestion.IsValidate() == false)
            {
                kingQuestion = null;
                return -1;
            }

            // 保存请求上下文
            var context = new KingContext
            {
                KingRequest = KingRequest.Parse(requestBody),
                QuestionData = kingQuestion.data
            };
            KingContextTable.Add(context);


            using (var sqlLite = new SqlliteContext())
            {
                var quiz = kingQuestion.data.quiz;
                var quizAnswer = sqlLite.QuizAnswer.FirstOrDefault(item => item.Quiz == quiz);

                if (quizAnswer != null)
                {
                    return Array.FindIndex(kingQuestion.data.options, item => item == quizAnswer.Answer);
                }

                // 搜索
                var best = BaiduSearcher.Search(kingQuestion).Best;
                if (best != null)
                {
                    quizAnswer = new QuizAnswer
                    {
                        Answer = best.Options,
                        Quiz = kingQuestion.data.quiz
                    };
                    sqlLite.QuizAnswer.Add(quizAnswer);
                    sqlLite.SaveChanges();
                }
                return best == null ? -1 : best.Index;
            }
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

            if (kingAnswer == null || kingAnswer.data == null)
            {
                return;
            }

            var context = KingContextTable.GetByRequest(kingRequest);
            if (context != null)
            {
                using (var sqlLite = new SqlliteContext())
                {
                    var quiz = context.QuestionData.quiz;
                    var quizAnswer = sqlLite.QuizAnswer.Find(quiz);

                    if (quizAnswer != null)
                    {
                        quizAnswer.Answer = context.GetAnswer(kingAnswer);
                        sqlLite.SaveChanges();
                    }
                }
            }
        }
    }
}
