using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy.Search
{
    /// <summary>
    /// 返回null结果的搜索引擎
    /// </summary>
    class NoResultSearchEngine : SearchEngineBase
    {
        public override BestOption Search(KingQuestion kingQuestion)
        {
            return null;
        }

        protected override string[] SearchSourceAnswers(string question)
        {
            return new string[0];
        }
    }
}
