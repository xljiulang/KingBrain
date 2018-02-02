using CsQuery;
using KingQuestionProxy.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    /// <summary>
    /// 提供提问的搜索
    /// 多搜索引擎
    /// </summary>
    static class Searcher
    {
        /// <summary>
        /// 搜索引擎
        /// </summary>
        private static readonly ISearchEngine searchEngine;

        /// <summary>
        /// 提问的搜索
        /// </summary>
        static Searcher()
        {
            var engines = new ISearchEngine[]
            {
                new BaiduSearchEngine{ MatchMode= MatchMode.Accurate },
                new BingSearchEngine{ MatchMode= MatchMode.Fuzzy },
                new ZhidaoSearchEngine{ MatchMode= MatchMode.Fuzzy },
                new NoResultSearchEngine()
            };

            engines.Aggregate((cur, next) =>
            {
                cur.Next = next;
                return next;
            });
            searchEngine = engines.FirstOrDefault();
        }

        /// <summary>
        /// 搜索问题
        /// </summary>
        /// <param name="question">问题</param>
        /// <returns></returns>
        public static BestOption Search(KingQuestion question)
        {
            return searchEngine.Search(question);
        }
    }
}
