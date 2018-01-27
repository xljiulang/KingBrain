using KingAnswerServer.Serarchers;
using KingQuestion;
using NetworkSocket.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingAnswerServer
{
    /// <summary>
    /// Http控制器
    /// </summary>
    class QuestionController : HttpController
    {
        /// <summary>
        /// 最近一次搜索结果
        /// </summary>
        private static SearchResult searchResult;

        /// <summary>
        /// 用于接收代理服务器转发的数据
        /// </summary>
        /// <param name="requestUrl">请求地址</param>
        /// <returns></returns>
        [HttpPost]
        [Route("/SendQuestionAsync")]
        public async Task<bool> SendQuestionAsync(string requestUrl)
        {
            // 只关注这个请求
            if (requestUrl.Contains("question/bat/findQuiz") == true)
            {
                // 服务器回复的内容
                var body = Encoding.UTF8.GetString(this.Request.Body);
                var question = Question.Parse(body);
                if (question == null || question.IsValidate() == false)
                {
                    return false;
                }
                await this.SearchQuestionAsync(question);
                return true;
            }
            else if (requestUrl.Contains("question/bat/choose") == true)
            {
                var body = Encoding.UTF8.GetString(this.Request.Body);
                var answer = Answer.Parse(body);

                if (answer != null && answer.data != null && searchResult != null)
                {
                    searchResult.UpdateBestAndSave(answer.data.answer - 1);
                    Console.WriteLine("本题正确的答案是：" + searchResult.Best.Options);
                    Console.WriteLine();
                }
            }
            return false;
        }

        /// <summary>
        /// 找答案并打印
        /// </summary>
        /// <param name="question">问题</param>
        /// <returns></returns>
        private async Task SearchQuestionAsync(Question question)
        {
            var title = question.data.quiz;
            var beginTime = DateTime.Now;
            Console.WriteLine(beginTime);
            Console.WriteLine(title);

            searchResult = SearchResult.SearchFromLocal(title);
            if (searchResult == null)
            {
                var searcher = new BaiduSearcher();
                searchResult = await searcher.GetAnswerOptions(question);
            }

            Console.WriteLine(searchResult.ToString(false));
            const double offsetSecondes = 3.7d;

            if (searchResult.Best != null)
            {
                var delay = beginTime.AddSeconds(offsetSecondes).Subtract(DateTime.Now);
                await this.AutotapOptionsAsync(searchResult.Best.Index, delay, TimeSpan.FromSeconds(1d));
            }
            else
            {
                // 8s后进入瞎蒙模式
                var delay = beginTime.AddSeconds(offsetSecondes + 8d).Subtract(DateTime.Now);
                var index = new Random().Next(0, 3);
                await this.AutotapOptionsAsync(index, delay);
            }

            Console.WriteLine();
        }

        /// <summary>
        /// 自动点击
        /// 这里要根据设备来调节
        /// 现在是硬编码
        /// </summary>
        /// <param name="optIndex"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private async Task AutotapOptionsAsync(int optIndex, TimeSpan delay, params TimeSpan[] delays)
        {
            var size = await adb.GetSizeAsync();
            if (size.IsEmpty == true)
            {
                return;
            }

            var x = size.Width / 2f;
            var uY = size.Height - 1095f;
            var buttonH = 1095f / 5f;
            var y = uY + buttonH * optIndex + buttonH / 4f;

            if (delay > TimeSpan.Zero)
            {
                Console.WriteLine("{0}秒后自动选择答案...", delay.TotalSeconds.ToString("0.0"));
                await Task.Delay(delay);
            }
            await adb.KeyeventAsync((int)x, (int)y);

            foreach (var item in delays)
            {
                await Task.Delay(item);
                await adb.KeyeventAsync((int)x, (int)y);
            }
        }
    }
}
