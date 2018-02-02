using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KingQuestionProxy.Search
{
    /// <summary>
    /// 表示搜索引擎抽象类
    /// </summary>
    public abstract class SearchEngineBase : ISearchEngine
    {
        /// <summary>
        /// 获取下一个引擎
        /// </summary>
        public ISearchEngine Next { get; private set; }

        /// <summary>
        /// 获取或设置匹配模式
        /// </summary>
        public MatchMode MatchMode { get; set; }

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
            var quiz = this.ClearNot(kingQuestion.data.quiz, out bool clearNot);
            var sourceAnswer = this.SearchSourceAnswers(quiz, trys: 1);
            if (sourceAnswer == null || sourceAnswer.Length == 0)
            {
                return this.Next.Search(kingQuestion);
            }

            // 各个选项和结论的匹配次数
            var opts = kingQuestion.data.options;
            var options = opts.Select((opt, i) => new
            {
                Index = i,
                Option = opt,
                Score = this.GetMatchScore(sourceAnswer, opt)
            }).ToArray();

            var best = clearNot ?
                options.OrderBy(item => item.Score).FirstOrDefault() :
                options.OrderByDescending(item => item.Score).FirstOrDefault();

            // 两个相同的结果，表示没有答案
            const int digits = 5;
            if (options.Any(item => item != best && Math.Round(item.Score, digits) == Math.Round(best.Score, digits)))
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
        /// 清除反语
        /// </summary>
        /// <param name="quiz"></param>
        /// <param name="clear"></param>
        /// <returns></returns>
        private string ClearNot(string quiz, out bool clear)
        {
            var state = false;
            quiz = Regex.Replace(quiz, @"不是|没有|不属于", m =>
            {
                state = true;
                return "是";
            });
            clear = state;
            return quiz;
        }

        /// <summary>
        /// 返回选项与原始答案的匹配度
        /// </summary>
        /// <param name="sourcesAnswers">原始答案</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        protected virtual decimal GetMatchScore(string[] sourcesAnswers, string options)
        {
            if (this.MatchMode == MatchMode.Accurate)
            {
                var fixOptions = options.Trim('《', '》', '<', '>').Trim();
                return sourcesAnswers.Count(item => item.IndexOf(fixOptions, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            else
            {
                return sourcesAnswers.Average(item => this.GetSimilarityWith(item, options));
            }
        }

        /// <summary>
        /// 获取和目标字符串的相似度
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="target">目标字符串</param>
        /// <returns></returns>
        private decimal GetSimilarityWith(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            {
                return 0m;
            }

            const decimal Kq = 2m;
            const decimal Kr = 1m;
            const decimal Ks = 1m;

            var sourceArray = source.ToCharArray();
            var destArray = target.ToCharArray();

            //获取交集数量
            var q = sourceArray.Intersect(destArray, CharComparer.Instance).Count();
            var s = sourceArray.Length - q;
            var r = destArray.Length - q;

            return Kq * q / (Kq * q + Kr * r + Ks * s);
        }


        /// <summary>
        /// 找class="c-abstract"的标签的文本
        /// 就是原始参数答案
        /// </summary>
        /// <param name="quiz"></param>
        /// <param name="trys">尝试次数</param>
        /// <returns></returns>
        private string[] SearchSourceAnswers(string quiz, int trys)
        {
            for (var i = 0; i <= trys; i++)
            {
                try
                {
                    return SearchSourceAnswers(quiz);
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
        /// <param name="quiz">问题</param>
        /// <returns></returns>
        protected abstract string[] SearchSourceAnswers(string quiz);

        /// <summary>
        /// 字符比较器
        /// </summary>
        private class CharComparer : IEqualityComparer<char>
        {
            /// <summary>
            /// 唯一实例
            /// </summary>
            public static readonly CharComparer Instance = new CharComparer();

            public bool Equals(char x, char y)
            {
                return char.ToUpper(x) == char.ToUpper(y);
            }

            public int GetHashCode(char obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
