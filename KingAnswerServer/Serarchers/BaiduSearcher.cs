using CsQuery;
using KingQuestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KingAnswerServer.Serarchers
{
    /// <summary>
    /// 百度搜索工具
    /// </summary>
    class BaiduSearcher : SerarcherBase
    {
        /// <summary>
        /// http客户端
        /// </summary>
        private static readonly HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(3d) };

        /// <summary>
        /// 找class="c-abstract"的标签的文本
        /// 就是原始参数答案
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        protected override async Task<string[]> SearchSourceAnswersAsync(string question)
        {
            httpClient.CancelPendingRequests();
            for (var i = 0; i < 3; i++)
            {
                try
                {
                    var address = $"http://www.baidu.com/s?ie=utf-8&wd={question}";
                    var datas = await httpClient.GetByteArrayAsync(address);
                    CQ cQ = Encoding.UTF8.GetString(datas);
                    var abstracts = cQ.Find(".c-abstract").Select(item => item.Cq().Text()).ToArray();
                    return abstracts;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return new string[0];
        }
    }
}
