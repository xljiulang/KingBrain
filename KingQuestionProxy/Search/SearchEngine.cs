using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy.Search
{
    /// <summary>
    /// 表示搜索引擎抽象类
    /// </summary>
    public abstract class SearchEngine : ISearchEngine
    {
        /// <summary>
        /// 获取下一个引擎
        /// </summary>
        public ISearchEngine Next { get; private set; }

        /// <summary>
        /// 设置下一个引擎
        /// </summary>

        ISearchEngine ISearchEngine.Next
        {
            set
            {
                this.Next = value;
            }
        }

        /// <summary>
        /// 搜索问题
        /// </summary>
        /// <param name="kingQuestion">问题</param>
        /// <returns></returns>
        public virtual BestOption Search(KingQuestion kingQuestion)
        {
            // 从badidu找出原始结论
            var title = kingQuestion.data.quiz;
            var sourceAnswer = this.SearchSourceAnswers(title, trys: 3);

            // 各个选项和结论的匹配次数
            var options = kingQuestion.data.options.Select((item, i) => new
            {
                Index = i,
                Option = item,
                Matchs = this.GetMatchCount(sourceAnswer, item)
            }).ToArray();

            var best = options.OrderByDescending(item => item.Matchs).FirstOrDefault();
            if (title.Contains("不") || title.Contains("没"))
            {
                // 计算匹配次数平均值，找出和匹配次数均值差异最大的
                var avg = options.Average(item => item.Matchs);
                best = options.OrderByDescending(item => Math.Pow(item.Matchs - avg, 2)).FirstOrDefault();
            }

            // 两个相同的结果，表示没有答案
            if (options.Any(item => item != best && item.Matchs == best.Matchs))
            {
                return this.Next.Search(kingQuestion);
            }

            return new BestOption
            {
                Index = best.Index,
                Option = best.Option
            };
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
        /// 找class="c-abstract"的标签的文本
        /// 就是原始参数答案
        /// </summary>
        /// <param name="question"></param>
        /// <param name="trys">尝试次数</param>
        /// <returns></returns>
        private string[] SearchSourceAnswers(string question, int trys)
        {
            for (var i = 0; i < trys; i++)
            {
                try
                {
                    return SearchSourceAnswers(question);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return new string[0];
        }

        /// <summary>
        /// 找原始参数答案
        /// </summary>
        /// <param name="question">问题</param>
        /// <returns></returns>
        protected abstract string[] SearchSourceAnswers(string question);
    }
}
