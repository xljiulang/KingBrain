using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    /// <summary>
    /// 百度搜索工具
    /// </summary>
    static class BaiduSearcher
    {
        /// <summary>
        /// http客户端
        /// </summary>
        private static readonly HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(3d) };

        /// <summary>
        /// 从正确的答案选项
        /// </summary>
        /// <param name="question">问题</param>
        /// <returns></returns>
        public static async Task<SearchResult> SearchAsync(KingQuestion question)
        {
            // 从badidu找出原始结论
            var title = question.data.quiz;
            var sourceAnswer = await SearchSourceAnswersAsync(title);

            // 各个选项和结论的匹配次数
            var options = question.data.options.Select((item, i) => new OptionMatchs
            {
                Index = i,
                Options = item,
                Matchs = GetMatchCount(sourceAnswer, item)
            }).ToArray();

            var best = options.OrderByDescending(item => item.Matchs).FirstOrDefault();
            if (title.Contains("不") || title.Contains("没"))
            {
                // 计算匹配次数平均值，找出和匹配次数均值差异最大的
                var avg = options.Average(item => item.Matchs);
                best = options.OrderByDescending(item => Math.Pow(item.Matchs - avg, 2)).FirstOrDefault();
            }

            var result = new SearchResult
            {
                Title = title,
                Options = options,
                Best = best
            };

            // 两个相同的结果，表示没有答案
            if (options.Any(item => item != best && item.Matchs == best.Matchs))
            {
                result.Best = null;
            }
            return result;
        }

        /// <summary>
        /// 返回选项与原始答案的匹配次数
        /// </summary>
        /// <param name="sourcesAnswers">原始答案</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        private static int GetMatchCount(string[] sourcesAnswers, string options)
        {
            var fixOptions = options.Trim('《', '》', '<', '>');
            return sourcesAnswers.Count(item => item.Contains(fixOptions));
        }

        /// <summary>
        /// 找class="c-abstract"的标签的文本
        /// 就是原始参数答案
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        private static async Task<string[]> SearchSourceAnswersAsync(string question)
        {
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
