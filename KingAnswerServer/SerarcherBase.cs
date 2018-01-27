using KingQuestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingAnswerServer
{
    /// <summary>
    /// 表示搜索工具基类
    /// </summary>
    public abstract class SerarcherBase
    {
        /// <summary>
        /// 从正确的答案选项
        /// </summary>
        /// <param name="question">问题</param>
        /// <returns></returns>
        public virtual async Task<SearchResult> GetAnswerOptions(Question question)
        {
            // 从badidu找出原始结论
            var title = question.data.quiz;
            var sourceAnswer = await this.SearchSourceAnswersAsync(title);

            // 各个选项和结论的匹配次数
            var options = question.data.options.Select((item, i) => new OptionMatchs
            {
                Index = i,
                Options = item,
                Matchs = this.GetMatchCount(sourceAnswer, item)
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
        protected virtual int GetMatchCount(string[] sourcesAnswers, string options)
        {
            var fixOptions = options.Trim('《', '》', '<', '>');
            return sourcesAnswers.Count(item => item.Contains(fixOptions));
        }

        /// <summary>
        /// 从网络查找相关的原始答案
        /// </summary>
        /// <param name="question">问题</param>
        /// <returns></returns>
        protected abstract Task<string[]> SearchSourceAnswersAsync(string question);
    }
}
